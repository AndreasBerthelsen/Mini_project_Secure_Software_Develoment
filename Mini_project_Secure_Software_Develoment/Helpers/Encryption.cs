using Konscious.Security.Cryptography;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Mini_project_Secure_Software_Develoment.Helpers
{
    public static class Encryption
    {

        public static byte[] DeriveKey(string masterPassword)
        {
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(masterPassword)))
            {
                argon2.DegreeOfParallelism = 1;
                argon2.Iterations = 2;
                argon2.MemorySize = 19456;

                return argon2.GetBytes(32);
            }
        }

        public static string EncryptString(string plainText, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = new byte[16];

                using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    using (var sw = new StreamWriter(cs))
                    {
                        sw.Write(plainText);

                    }
                    return Convert.ToBase64String(ms.ToArray());
                }

            }
        }

        public static string DecryptString(string cipherText, byte[] key)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = new byte[16];

                using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
                {

                    using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    using (var sr = new StreamReader(cs))
                    {

                        return sr.ReadToEnd();

                    }
                }
            }
        }
    }
}
