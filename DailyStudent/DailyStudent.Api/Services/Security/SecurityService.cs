using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using DailyStudent.Api.Constants;
using DailyStudent.Api.DataAccess;
using DailyStudent.Api.Services.Security.UserContext;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace DailyStudent.Api.Services.Security
{
    public class SecurityService: ISecurityService
    {
        
        private readonly DailyStudentDbContext _context;

        public SecurityService(DailyStudentDbContext context, IUserContext _dUserContext)
        {
            _context = context;
        }

        public string GenerateJWT(string email)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var key = System.Text.Encoding.ASCII.GetBytes(Settings.JWTPrivateKey);

            var claims = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme); //TODOD: revisar lo de los schemas

            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, email));
            claims.AddClaim(new Claim(ClaimTypes.Name, email));
            claims.AddClaim(new Claim(ClaimTypes.Role, "User"));

            var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
            {
                Subject = claims,
                //Expires = _ts.ExpireTime,
                SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

        public async Task<SessionUser> GetUserInstance(string email) //TODO: evaluar si poner id
        {
            //TODO: ver como obtener la ip aca
           // string ip = "";
            var user = await _context.User.Include(x => x.Userinfo).AsNoTracking().SingleOrDefaultAsync(x => x.Email == email);

            if (user == null)
                return null;

            //var userDevice = await _context.UserLoginDevice.AsNoTracking().SingleOrDefaultAsync(x => x.UserId == user.Id && x.Ip == ip);

            //if (userDevice == null) // Verifica que el usuario esta logeado en este dispositivo
            //    return null;

            var userInfo = user.Userinfo.FirstOrDefault();

            return new SessionUser
            {
                Id = user.Id,
                Name = userInfo.FirstName,
                LastName = userInfo.LastName,
                ProfileImage = userInfo.ImagePath,
                Email = user.Email,
                Rol = user.UserRolId
            };
        }

        public SessionUser ValidateIdentity(IIdentity identity)
        {
            var userInstance = GetUserInstance(identity.Name).Result;

            // Verify user exists
            if (userInstance == null)
                return null;

            //this.CurrentUser = userInstance;

            return userInstance;
        }

    }
}