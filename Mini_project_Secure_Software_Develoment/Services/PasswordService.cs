using Mini_project_Secure_Software_Develoment.Model;
using Mini_project_Secure_Software_Develoment.Repositories.Interfaces;
using Mini_project_Secure_Software_Develoment.Services;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Mini_project_Secure_Software_Develoment.Helpers
{
    public class PasswordService
    {
        private readonly IPasswordRepo _passwordRepo;
        private readonly MasterPasswordService _masterPasswordService;
        private readonly EncryptionService _encryptionService;

        public PasswordService (IPasswordRepo passwordRepo, MasterPasswordService masterPasswordService, EncryptionService encryptionService)
        {
            _passwordRepo = passwordRepo;
            _masterPasswordService = masterPasswordService;
            _encryptionService = encryptionService;
        }

        public async Task AddingEncryption(PasswordEntry passwordEntry)
        {
            var key = _encryptionService.Key;
            var encryptPassword = Encryption.EncryptString(passwordEntry.Password, key );

            var passwordDTO = new PasswordEntry
            {
                App = passwordEntry.App,
                Password = encryptPassword,
                Salt = GenerateSalt()

            };
            await _passwordRepo.AddPasswordAsync(passwordDTO);
        }

        public byte[] GenerateSalt()
        {
            var salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
            var keysalt = Convert.FromBase64String(salt);
            return keysalt;
        }

        public async Task DeletePasswordAsync(int id)
        {

            var listOfPasswords = await _passwordRepo.GetAllPasswordsAsync();

            foreach (var password in listOfPasswords)
            {
                if (password.Id == id)
                {
                    _passwordRepo.DeletePasswordAsync(password);
                }
            }
        }

        public async Task<List<PasswordEntry>> GetAllPasswordsAsync()
        {
            var key = _encryptionService.Key;
            
            var result = new List<PasswordEntry>();
            
            var encryptedPasswords = await _passwordRepo.GetAllPasswordsAsync();
            
            foreach (var password in encryptedPasswords)
            {
                var passwordDTO = new PasswordEntry
                {
                    Id = password.Id,
                    App = password.App,
                    Password = Encryption.DecryptString(password.Password, key),
                };
                result.Add(passwordDTO);
            }
            return result;
        }
    }
}