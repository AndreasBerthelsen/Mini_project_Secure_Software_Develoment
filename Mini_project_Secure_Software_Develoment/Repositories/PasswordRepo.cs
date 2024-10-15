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
    public class PasswordRepo : IPasswordRepo
    {
        private readonly PasswordManagerContext _context;

        public PasswordRepo(PasswordManagerContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        public async Task AddPasswordAsync(PasswordEntry password)
        {
            _context.Passwords.Add(password);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePasswordAsync(PasswordEntry password)
        {
            _context.Passwords.Remove(password);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PasswordEntry>> GetAllPasswordsAsync()
        {
            return await _context.Passwords.ToListAsync();
        }

        public async Task<PasswordEntry> GetPasswordAsync(int id)
        {
            var password = _context.Passwords.FirstOrDefault(p => p.Id == id);
            return password;
        }
    }
}
