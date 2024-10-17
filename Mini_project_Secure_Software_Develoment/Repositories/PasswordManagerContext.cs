using Microsoft.EntityFrameworkCore;
using Mini_project_Secure_Software_Develoment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_project_Secure_Software_Develoment.Repositories
{
    public class PasswordManagerContext : DbContext
    {
        public PasswordManagerContext(DbContextOptions<PasswordManagerContext> options)
           : base(options)
        {
        }

        public DbSet<PasswordEntry> Passwords { get; set; }
    }
}
