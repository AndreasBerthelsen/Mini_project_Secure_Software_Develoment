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
        private byte[] _key;

        public byte[] Key => _key;

        public async Task ResetKey()
        {
            _key = null;
        }

        public async Task InitializeAsync(string masterPassword)
        {
            _key = Encryption.DeriveKey(masterPassword);
        }
    }
}
