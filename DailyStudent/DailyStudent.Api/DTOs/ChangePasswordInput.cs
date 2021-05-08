using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.DTOs
{
    public class ChangePasswordInput
    {
        public string Tk { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string RepeatPassword { get; set; }
    }
}
