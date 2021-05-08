using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyStudent.Api.Constants;
using DailyStudent.Api.DTOs;

namespace DailyStudent.Api.Services.Account
{
    public interface IAccountService
    {
        Task<List<UserOutput>> GetUsers(string rol);
        Task SignUp(SignUpUser.Input input);
        Task<SignInUser.Output> SignIn(SignInUser.Input input);
        Task ToggleState(int userId);
        Task SignOut();
        Task ForgotPassword(string email, Applications app);
        Task<dynamic> UpdateCurrentAccount(UserUpdateInput user);
        Task ChangePassoword(ChangePasswordInput model);
        Task ChangeCurrentPassword(ChangePasswordInput input);
        Task<bool> VerifyUser(string tk);
    }
}