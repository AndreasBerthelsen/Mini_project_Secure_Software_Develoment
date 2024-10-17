using Microsoft.IdentityModel.Tokens;
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
        private readonly EncryptionService _encryptionService;
        private readonly IPasswordRepo _passwordRepo;

        public MasterPasswordService(EncryptionService encryptionService, IPasswordRepo passwordRepo )
        {
            _encryptionService = encryptionService;
            _passwordRepo = passwordRepo;
        }
        
        public async Task SetMasterPasswordAsync(string password)
        {

            var masterPassword = new MasterPassword
            {
                Password = password,
            };
            await _encryptionService.InitializeAsync(masterPassword.Password);
        }

        public async Task<bool> ValidateMasterPasswordAsync(string password)
        {
            var passwords = await _passwordRepo.GetAllPasswordsAsync();
            try
            {
                Encryption.DecryptString(passwords[0].Password, Encryption.DeriveKey(password));


                await _encryptionService.InitializeAsync(password);

                return true;
            } catch (Exception ex)
            {
                return false;
            }
            
        }


    }
}
