using Mini_project_Secure_Software_Develoment.Helpers;
using Mini_project_Secure_Software_Develoment.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_project_Secure_Software_Develoment.Services
{
    public class EncryptionService
    {
        private readonly IMasterPasswordRepo _repo;
        private byte[] _key;

        public byte[] Key => _key;

        public EncryptionService(IMasterPasswordRepo repo)
        {
            _repo = repo;
        }

        public async Task InitializeAsync(string masterPassword, byte[] salt)
        {
            _key = Encryption.DeriveKey(masterPassword, salt);
        }
    }
}
