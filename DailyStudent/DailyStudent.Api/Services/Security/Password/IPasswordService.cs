using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Security.Password
{
    public interface IPasswordsService
    {
        EncryptedPassword Encrypt(string password);
        bool Verify(byte[] hashedPassword, string passwordSalt, string password);
    }
}