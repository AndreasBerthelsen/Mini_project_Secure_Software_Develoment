using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;    
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_project_Secure_Software_Develoment.Repositories
{
    public class PasswordManagerContextFactory : IDesignTimeDbContextFactory<PasswordManagerContext>
    {
        public PasswordManagerContext CreateDbContext(string[] args)
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<PasswordManagerContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new PasswordManagerContext(optionsBuilder.Options);
        }

    }
}
