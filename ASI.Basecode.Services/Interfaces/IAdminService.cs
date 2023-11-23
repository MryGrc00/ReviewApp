using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Interfaces
{
    public interface IAdminService
    {
        List<AdminViewModel> GetAdmins();
        AdminViewModel PaginatedAdmins(int page, int pageSize); 
        AdminViewModel GetAdmin(int id);
        AdminViewModel GetAdminWithPassword(int id);
        AdminViewModel GetAdminWithEmail(string email);
        void AddAdmin(AdminViewModel model, string name);
        string CheckEmail(string email);
        string CheckPasswords(string password, string confirmPassword);
        void UpdateAdmin(AdminViewModel model, string name);
        void DeleteAdmin(AdminViewModel model);
    }
}
