using System.ComponentModel.DataAnnotations;
using DailyStudent.Api.Services.Security;
using DailyStudent.Api.Services.Security.UserContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DailyStudent.Api.Services.Extensions
{
    public class AccessRolAttribute : ActionFilterAttribute
    {
        public string Rol { get; private set; }

        public AccessRolAttribute(string rol)
        {
            Rol = rol;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var userContext = (IUserContext)context.HttpContext.RequestServices.GetService(typeof(IUserContext));
            if (userContext.User.Rol != Rol)
                context.Result = new UnauthorizedObjectResult("Acceso denegado, Rol no autorizado"); //TODO: manejo de mensajes

        }
    }
}