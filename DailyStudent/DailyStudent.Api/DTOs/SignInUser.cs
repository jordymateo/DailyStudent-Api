using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyStudent.Api.Constants;

namespace DailyStudent.Api.DTOs
{
    public class SignInUser
    {
        public class Input
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string Ip { get; set; }
            public string Location { get; set; }
            public Applications App { get; set; }
        }

        public class Output
        {
            public string Token { get; set; }
        }
    }
}