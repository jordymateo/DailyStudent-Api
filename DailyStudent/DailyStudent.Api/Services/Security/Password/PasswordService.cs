using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Security.Password
{
    public class PasswordsService: IPasswordsService
    {
        private const int SaltSize = 16; // 128 bit 
        private const int KeySize = 32; // 256 bit
        private const int Iterations = 1000;

        public EncryptedPassword Encrypt(string password)
        {
            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              SaltSize,
              Iterations,
              HashAlgorithmName.SHA512))
            {
                var salt = Convert.ToBase64String(algorithm.Salt);

                return new EncryptedPassword
                {
                    Password = algorithm.GetBytes(KeySize),
                    Salt = salt + '.' + Iterations
                };
            }
        }

        public bool Verify(byte[] hashedPassword, string passwordSalt, string password)
        {
            var parts = passwordSalt.Split('.', 2);

            if (parts.Length != 2)
            {
                throw new FormatException("Unexpected hash format. " +
                  "Should be formatted as `{iterations}.{salt}`");
            }

            var salt = Convert.FromBase64String(parts[0]);
            var iterations = Convert.ToInt32(parts[1]);


            using (var algorithm = new Rfc2898DeriveBytes(
              password,
              salt,
              iterations,
              HashAlgorithmName.SHA512))
            {
                return algorithm.GetBytes(KeySize).SequenceEqual(hashedPassword);
            }
        }
    }
}