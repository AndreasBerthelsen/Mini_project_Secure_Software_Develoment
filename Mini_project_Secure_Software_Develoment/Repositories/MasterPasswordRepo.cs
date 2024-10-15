using Microsoft.EntityFrameworkCore;
using Mini_project_Secure_Software_Develoment.Model;
using Mini_project_Secure_Software_Develoment.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_project_Secure_Software_Develoment.Repositories
{
    public class MasterPasswordRepo : IMasterPasswordRepo
    {
        private readonly PasswordManagerContext _context;
        public MasterPasswordRepo(PasswordManagerContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        public async Task AddMasterPasswordAsync(MasterPassword masterPassword)
        {

            _context.MasterPasswords.Add(masterPassword);
            await _context.SaveChangesAsync();

        }

        public async Task DeleteMasterPasswordAsync(int id)
        {
            var masterPassword = await GetMasterPasswordAsync(); // Await here

            if (masterPassword != null && masterPassword.Id == id)
            {
                _context.MasterPasswords.Remove(masterPassword);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<MasterPassword> GetMasterPasswordAsync()
        {
            return await _context.MasterPasswords.FirstOrDefaultAsync();
        }
    }
}
