using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using DailyStudent.Api.Constants;
using DailyStudent.Api.DataAccess;
using DailyStudent.Api.DTOs;
using DailyStudent.Api.Exceptions;
using DailyStudent.Api.Services.Cloud;
using DailyStudent.Api.Services.Messages;
using DailyStudent.Api.Services.Security;
using DailyStudent.Api.Services.Security.Password;
using DailyStudent.Api.Services.Security.UserContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DailyStudent.Api.Services.Account
{
    public class AccountService: IAccountService
    {
        private readonly IUserContext _userContext;
        private readonly DailyStudentDbContext _context;
        private readonly IGoogleCloudService _cloudService;
        private readonly ISecurityService _securityService;
        private readonly MessagesOptions _messagesOptions;
        private readonly IPasswordsService _passwordsService;


        public AccountService(
            IUserContext userContext,
            DailyStudentDbContext context,
            IGoogleCloudService cloudService,
            ISecurityService securityService,
            IPasswordsService passwordsService,
            IOptions<MessagesOptions> messagesOptions
            )
        {
            _context = context;
            _userContext = userContext;
            _cloudService = cloudService;
            _securityService = securityService;
            _passwordsService = passwordsService;
            _messagesOptions = messagesOptions.Value;
        }

        public async Task SignUp(SignUpUser.Input input)
        {
            IDbContextTransaction transaction = null;

            try
            {
                if (input is null)
                    throw new ArgumentNullException(nameof(input)); 

                if (string.IsNullOrWhiteSpace(input.Email))
                    throw new Exception("Se requiere el campo" + nameof(input.Email));

                if (_context.User.Any(x => x.Email == input.Email))
                    throw new MessageException(10);

                var pass = _passwordsService.Encrypt(input.Password);

                var user = new User
                {
                    Email = input.Email,
                    Password = pass.Password,
                    PasswordSalt = pass.Salt,
                    UserRolId = input.UserRolId,
                    IsBloqued = true
                };

                transaction = _context.Database.BeginTransaction();

                await _context.User.AddAsync(user);
                
                _context.SaveChanges();

                string imageUrl = null;

                if (input.Image != null)
                    imageUrl = await _cloudService.SaveProfileImage(user.Id, input.Image);

                var userInfo = new UserInfo
                {
                    UserId = user.Id,
                    FirstName = input.FirstName,
                    LastName = input.LastName,
                    CreationDate= DateTime.Now,
                    ImagePath = imageUrl
                };

                _context.UserInfo.Add(userInfo);

                await _context.SaveChangesAsync();

                string body = @"<strong>¡Bienvenido!</strong> al sistema que facilitará la gestion de tu contenido estudiantil.
                Estas a un paso de completar el proceso de registro, haz click en el siguiente botón
                para confirmar tu dirección de correo eléctronico:";

                var jwt = _securityService.GenerateJWT(input.Email);

                if (!SendEMail(input.Email, "Verificación de correo eléctronico", body, "/v/" + jwt))
                    throw new MessageException(11);

                transaction.Commit();
            }
            catch (Exception ex)
            {
                if (transaction != null) 
                {
                    transaction.Rollback();
                    transaction.Dispose();
                }
                //TODO: Implement log
                throw ex;
            }
            finally
            {

                if (transaction != null)
                    transaction.Dispose();
            }
        }


        /// <summary>
        /// Sign in user to application giving him a JWT
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns>User JWT</returns>
        public async Task<SignInUser.Output> SignIn(SignInUser.Input input)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Email == input.Email);

            if (user == null || !_passwordsService.Verify(user.Password, user.PasswordSalt, input.Password))
                throw new Exception("El email o la contraseña es incorrecto");

            if (input.App == Constants.Applications.AdminsApp && user.UserRolId != "admin")
                throw new MessageException(5);

            if (user.IsBloqued)
                throw new MessageException(13);

            var jwt = _securityService.GenerateJWT(input.Email);

            return new SignInUser.Output
            {
                Token = jwt
            };
        }

        public async Task<List<UserOutput>> GetUsers(string rol)
        {
            return await _context.User
                .Join(_context.UserInfo,
                u => u.Id,
                i => i.UserId,
                (u, i) => new { User = u, Info = i})
                .Where(x => x.User.UserRolId == rol)
                .Select(x => new UserOutput
                {
                    Id = x.User.Id,
                    Email = x.User.Email,
                    Name = x.Info.FirstName,
                    LastName = x.Info.LastName,
                    CreationDate = x.Info.CreationDate,
                    State = x.User.IsBloqued ? "Inactivo" : "Activo"
                })
                .ToListAsync();
        }

        public async Task ToggleState(int userId)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Id == userId);

            if (user == null)
                throw new MessageException(4, nameof(userId), userId.ToString());

            user.IsBloqued = !user.IsBloqued;

            await _context.SaveChangesAsync();
        }

        public async Task ForgotPassword(string email, Applications app)
        {
            var user = await _context.User.SingleOrDefaultAsync(x => x.Email.Trim() == email.Trim());

            if (user == null)
                throw new MessageException(14);

            string body = "Se ha recibido una solicitud para el cambio de contraseña de su cuenta. Si usted no ha realizado esta solicitud," +
                            " favor ignorar el correo lo contrario hacer clic en el botón de abajo.";

            var jwt = _securityService.GenerateJWT(email);

            if (!SendEMail(email, "Solicitud de cambio de contraseña", body, "/cp/" + jwt, app))
                throw new MessageException(11);
        }

        public async Task<bool> VerifyUser(string tk)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(Settings.JWTPrivateKey);

            try
            {
                handler.ValidateToken(tk, new TokenValidationParameters
                {
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.First(x => x.Type == "nameid").Value;

                var user = _context.User.SingleOrDefault(x => x.Email.Trim() == email.Trim());
                if (!user.IsBloqued)
                    return false;

                user.IsBloqued = false;

                await _context.SaveChangesAsync();
            } 
            catch (Exception ex)
            {
                throw new MessageException(12);
            }

            return true;
        }

        public async Task<dynamic> UpdateCurrentAccount(UserUpdateInput user)
        {
            var currentSessionUser = _userContext.User;

            var currentUser = _context.UserInfo.SingleOrDefault(user => user.UserId == currentSessionUser.Id);

            if (currentUser is null)
                throw new MessageException(2);

            currentUser.FirstName = user.Name;
            currentUser.LastName = user.LastName;

            string imageUrl = null;

            if (user.Image != null)
            {
                imageUrl = await _cloudService.SaveProfileImage(currentUser.UserId, user.Image);
                currentUser.ImagePath = imageUrl;
            }

            await _context.SaveChangesAsync();

            return new
            {
                user.Name,
                user.LastName,
                _userContext.User.Email,
                ProfileImage = imageUrl
            };
        }
        public async Task ChangePassoword(ChangePasswordInput model)
        {
            var handler = new JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(Settings.JWTPrivateKey);

            try
            {
                handler.ValidateToken(model.Tk, new TokenValidationParameters
                {
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.First(x => x.Type == "nameid").Value;

                var user = _context.User.SingleOrDefault(x => x.Email.Trim() == email.Trim());

                if (model.NewPassword != model.RepeatPassword)
                    throw new MessageException(8);

                var pass = _passwordsService.Encrypt(model.NewPassword);

                user.Password = pass.Password;
                user.PasswordSalt = pass.Salt;

                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (ex is MessageException)
                    throw;

                throw new MessageException(15);
            }
        }

        public async Task ChangeCurrentPassword(ChangePasswordInput input)
        {
            var currentSessionUser = _userContext.User;

            var currentUser = _context.User.SingleOrDefault(user => user.Id == currentSessionUser.Id);

            if (currentUser is null)
                throw new MessageException(2);


            if (!_passwordsService.Verify(currentUser.Password, currentUser.PasswordSalt, input.CurrentPassword))
                throw new MessageException(7);

            if (input.NewPassword != input.RepeatPassword)
                throw new MessageException(8);


            var pass = _passwordsService.Encrypt(input.NewPassword);

            currentUser.Password = pass.Password;
            currentUser.PasswordSalt = pass.Salt;

            await _context.SaveChangesAsync();

        }

        public async Task SignOut()
        {
            //TODO: Obtener ip
            string ip = "";
            //var userDevice = await _context.UserLoginDevice.SingleOrDefaultAsync(x => x.UserId == _securityService.CurrentUser.Id && x.Ip == ip);

            //if (userDevice != null)
            //    _context.Remove(userDevice);

            //_context.SaveChanges();
        }


        private bool SendEMail(string emailTo, string subject, string body, string urlLink)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", _messagesOptions.TemplateName);

            string content = System.IO.File.ReadAllText(file);

            MailMessage message = new MailMessage();
            message.To.Add(emailTo);
            message.Subject = subject;
            message.Body = string.Format(content, body, _messagesOptions.FrontURL + urlLink);
            message.From = new MailAddress(_messagesOptions.EmailFrom);
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient("smtp-mail.outlook.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_messagesOptions.EmailFrom, _messagesOptions.EmailFromPassword),
                EnableSsl = true
            };

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        private bool SendEMail(string emailTo, string subject, string body, string urlLink, Applications app)
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", _messagesOptions.TemplateName);

            string content = System.IO.File.ReadAllText(file);

            MailMessage message = new MailMessage();
            message.To.Add(emailTo);
            message.Subject = subject;
            string frontUrl = app == Applications.AdminsApp ? _messagesOptions.FrontURLAdmin : _messagesOptions.FrontURL;
            message.Body = string.Format(content, body, frontUrl + urlLink);
            message.From = new MailAddress(_messagesOptions.EmailFrom);
            message.IsBodyHtml = true;

            SmtpClient client = new SmtpClient("smtp.office365.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(_messagesOptions.EmailFrom, _messagesOptions.EmailFromPassword),
                EnableSsl = true
            };

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return true;
        }
    }
}