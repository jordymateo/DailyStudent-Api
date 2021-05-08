using System.Text;
using System.Threading.Tasks;
using DailyStudent.Api.Constants;
using DailyStudent.Api.Services.Account;
using DailyStudent.Api.Services.Messages;
using DailyStudent.Api.Services.Security;
using DailyStudent.Api.Services.Security.Password;
using DailyStudent.Api.Services.Security.UserContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DailyStudent.Api.Services.Extensions
{
    public static class SecurityExtensions
    {
        public static void UseDailyStudentAuthentication(this IServiceCollection services)
        {

            services.AddTransient<IPasswordsService, PasswordsService>();
            services.AddTransient<ISecurityService, SecurityService>();
            services.AddTransient<IAccountService, AccountService>();
            services.AddSingleton<IUserContext, UserContext>();

            var authenticationBuilder = services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            var privateKey = Encoding.ASCII.GetBytes(Settings.JWTPrivateKey);

            authenticationBuilder.AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = (context) =>
                    {
                        var securityService = context.HttpContext.RequestServices.GetRequiredService<ISecurityService>();
                        var userInstance = securityService.ValidateIdentity(context.Principal.Identity);

                        var userContext = context.HttpContext.RequestServices.GetRequiredService<IUserContext>();

                        if (userInstance == null)
                        {
                            context.NoResult();
                            context.Response.StatusCode = 401;
                            context.Response.ContentType = "application/json";

                            var errorDetail = new
                            {
                                StatusCode = System.Net.HttpStatusCode.Unauthorized,
                                Message = "Access denied" //TODO: Si va a ser multilenguaje los mensajes deberian eestar parametrizados por idioma
                            };

                            //var accountService = context.HttpContext.RequestServices.GetRequiredService<IAccountService>();
                            //accountService.SignOut();
                            userContext.Clear();


                            context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(errorDetail)).Wait();

                        }
                        else
                        {
                            userContext.UpdateData(userInstance);
                        }

                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = (context) =>
                    {
                        context.NoResult();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";

                        var errorDetail = new
                        {
                            StatusCode = System.Net.HttpStatusCode.Unauthorized,
                            Message = "Access denied" //TODO: Si va a ser multilenguaje los mensajes deberian eestar parametrizados por idioma
                        };

                        context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(errorDetail)).Wait();

                        return Task.CompletedTask;
                    },
                    // OnMessageReceived = (context) => //TODO: por si se usa SignalR para las notificaciones
                    // {
                    //     var accessToken = context.Request.Query["access_token"];

                    //     var path = context.HttpContext.Request.Path;
                    //     if (!string.IsNullOrEmpty(accessToken) &&
                    //         (path.StartsWithSegments("/hubs")))
                    //     {
                    //         context.Token = accessToken;
                    //     }

                    //     return Task.CompletedTask;
                    // }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(privateKey),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        }
    }
}