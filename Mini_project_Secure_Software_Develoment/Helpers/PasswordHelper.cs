using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Mini_project_Secure_Software_Develoment.Helpers
{
    public class PasswordHelper
    {
        public static byte[] GenerateSalt(int size = 16)
        {
            using var random = RandomNumberGenerator.Create();
            byte[] salt = new byte[size];
            random.GetBytes(salt);
            return salt;
        }

        public static async Task<string> HashPassword(string password, byte[] salt)
        {
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 1,
                Iterations = 2,
                MemorySize = 19456,
            };
            byte[] hash = await argon2.GetBytesAsync(32);
            return Convert.ToBase64String(hash);
        }

        public static async Task<bool> VerifyPassword(string passwordInput, string storedHash, byte[] salt)
        {
            string hashing = await HashPassword(passwordInput, salt);
            Console.WriteLine(hashing);
            return hashing == storedHash;
        }

    }
}
