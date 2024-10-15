using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mini_project_Secure_Software_Develoment.Model
{
    public class PasswordEntry
    {
        public int Id { get; set; }
        public string App { get; set; }
        public string Password { get; set; }
        public byte[] Salt { get; set; }
    }
}
