using ASI.Basecode.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Interfaces
{
    public interface IAdminRepository
    {
        IQueryable<Admin> GetAdmins();
        Admin GetAdmin(int id);
        void AddAdmin(Admin admin);
        void EditAdmin(Admin admin);
        void DeleteAdmin(Admin admin);
    }
}
