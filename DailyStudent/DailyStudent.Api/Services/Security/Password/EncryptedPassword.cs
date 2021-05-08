using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Security.Password
{
    public class EncryptedPassword
    {
        public byte[] Password { get; set; }
        public string Salt { get; set; }
    }
}