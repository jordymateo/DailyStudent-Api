using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyStudent.Api.Constants;
using DailyStudent.Api.DTOs;
using DailyStudent.Api.Services.Account;
using DailyStudent.Api.Services.Extensions;
using DailyStudent.Api.Services.Security;
using DailyStudent.Api.Services.Security.UserContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DailyStudent.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserContext _userContext;
        private readonly IAccountService _accountService;

        public AccountController(
            IUserContext userContext,
            IAccountService accountService
        )
        { 
            _userContext = userContext;
            _accountService = accountService;
        }

        [HttpPost("SignIn")]
        public async Task<IActionResult> SignIn(SignInUser.Input user)
        {
            var output = await _accountService.SignIn(user);
            return Ok(output);
        }

        [HttpGet("Info")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Info()
        {
            return Ok(new
            {
                _userContext.User.Name,
                _userContext.User.LastName,
                _userContext.User.Email,
                _userContext.User.ProfileImage
            });
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromForm] SignUpUser.Input user)
        {
            user.UserRolId = UserRoles.Student;
            await _accountService.SignUp(user);
            return Ok();
        }

        [HttpPost("Admin/SignUp")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> SignUpAnAdmin([FromBody] SignUpUser.Input user)
        {
            user.UserRolId = UserRoles.Administrator;
            await _accountService.SignUp(user);
            return Ok();
        }

        [HttpGet("Students")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> GetStudents()
        {
            var users = await _accountService.GetUsers(UserRoles.Student);
            return Ok(users);
        }

        [HttpGet("Admins")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> GetAdmins()
        {
            var users = await _accountService.GetUsers(UserRoles.Administrator);
            return Ok(users);
        }

        [HttpPut("ToggleState/{userId}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [AccessRol(UserRoles.Administrator)]
        public async Task<IActionResult> ToggleState(int userId)
        {
            await _accountService.ToggleState(userId);
            return Ok();
        }

        [HttpPost("ForgotPassword/{email}/{app}")]
        public async Task<IActionResult> ForgotPassword(string email, Applications app = Applications.StudentsApp)
        {
            await _accountService.ForgotPassword(email, app);
            return Ok();
        }


        [HttpPut("UpdateCurrentAccount")]
        public async Task<IActionResult> UpdateCurrentAccount([FromForm] UserUpdateInput user)
        {
           var data = await _accountService.UpdateCurrentAccount(user);
            return Ok(data);
        }

        [HttpPut("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordInput input)
        {
            await _accountService.ChangePassoword(input);
            return Ok();
        }

        [HttpPut("ChangeCurrentPassword")]
        public async Task<IActionResult> ChangeCurrentPassword([FromBody] ChangePasswordInput input)
        {
            await _accountService.ChangeCurrentPassword(input);
            return Ok();
        }

        [HttpPut("VerifyUser/{tk}")]
        public async Task<IActionResult> VerifyUser(string tk)
        {
            var result = await _accountService.VerifyUser(tk);
            return Ok(result);
        }

        [HttpPost("SignOut")]
        public async Task<IActionResult> SignOut()
        {
            await _accountService.SignOut();
            return Ok();
        }
    }
}