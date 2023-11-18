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
        AdminViewModel GetAdmin(int id);
        void AddAdmin(AdminViewModel model, string name);
        bool CheckEmail(string email);
        void UpdateAdmin(AdminViewModel model, string name);
        void DeleteAdmin(AdminViewModel model);
    }
}
