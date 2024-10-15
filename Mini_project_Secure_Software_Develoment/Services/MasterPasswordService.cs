using Mini_project_Secure_Software_Develoment.Helpers;
using Mini_project_Secure_Software_Develoment.Model;
using Mini_project_Secure_Software_Develoment.Repositories;
using Mini_project_Secure_Software_Develoment.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_project_Secure_Software_Develoment.Services
{
    public class MasterPasswordService 
    {
        private readonly IMasterPasswordRepo _repo;
        private readonly EncryptionService _encryptionService;

        public MasterPasswordService(IMasterPasswordRepo repo, EncryptionService encryptionService )
        {
            _repo = repo;
            _encryptionService = encryptionService;
        }
        
        public async Task<bool> IsMasterPasswordSetAsync()
        {
            var masterPassword = await _repo.GetMasterPasswordAsync();
            return masterPassword != null;
        }

        public async Task SetMasterPasswordAsync(string password)
        {
            byte[] keySalt = PasswordHelper.GenerateSalt();

            string hashedMasterPassword = await PasswordHelper.HashPassword(password, keySalt);

            var masterPassword = new MasterPassword
            {
                Password = hashedMasterPassword,
                KeySalt = Convert.ToBase64String(keySalt),
            };
            await _encryptionService.InitializeAsync(password, keySalt);
            await _repo.AddMasterPasswordAsync(masterPassword);
        }

        public async Task<bool> ValidateMasterPasswordAsync(string password)
        {
            var masterPassword = await _repo.GetMasterPasswordAsync();
            if (masterPassword == null)
            {
                return false;
            }

            byte[] KeySalt = Convert.FromBase64String(masterPassword.KeySalt);

            if (await PasswordHelper.VerifyPassword(password, masterPassword.Password, KeySalt))
            {
                await _encryptionService.InitializeAsync(password, KeySalt);
                return true;
            }

            return false;
        }

        public async Task<string> GetHashedMasterPasswordAsync()
        {
            var masterPassword = await _repo.GetMasterPasswordAsync();
            if (masterPassword == null)
            {
                throw new InvalidOperationException("Master password not set.");
            }

            return masterPassword.Password; // Return the hashed password
        }

        public async Task<MasterPassword> GetMasterPasswordAsync()
        {
            return await _repo.GetMasterPasswordAsync();
        }

        

        public async Task DeleteMasterPasswordAsync(int id)
        {
            var masterPasswordToDelete = await GetMasterPasswordAsync(); // Await here
            if (masterPasswordToDelete != null && masterPasswordToDelete.Id == id)
            {
                await _repo.DeleteMasterPasswordAsync(masterPasswordToDelete.Id);
            }
            else
            {
                throw new InvalidOperationException("Master password not found or ID mismatch.");
            }
        }

    }
}
