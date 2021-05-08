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
    public interface ISecurityService
    {
        string GenerateJWT(string email);
        Task<SessionUser> GetUserInstance(string email);
        SessionUser ValidateIdentity(IIdentity identity);
    }
}