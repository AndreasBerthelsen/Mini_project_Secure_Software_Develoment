using Mini_project_Secure_Software_Develoment.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_project_Secure_Software_Develoment.Repositories.Interfaces
{
    public interface IMasterPasswordRepo
    {
        Task<MasterPassword> GetMasterPasswordAsync();
        Task AddMasterPasswordAsync(MasterPassword masterPassword);
        Task DeleteMasterPasswordAsync(int id);
    }
}
