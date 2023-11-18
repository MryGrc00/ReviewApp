using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Services.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public List<AdminViewModel> GetAdmins()
        {
            var data = _adminRepository.GetAdmins().Select(x => new AdminViewModel
            {
                AdminId = x.AdminId,
                Email = x.Email,
                Password = x.Password,
                Name = x.Name,
                CreatedBy = x.CreatedBy,
                CreatedTime = x.CreatedTime,
                UpdatedBy = x.UpdatedBy,
                UpdatedTime = x.UpdatedTime,
            }).OrderByDescending(x => x.CreatedTime).ToList();
            return data;
        }

        public AdminViewModel GetAdmin(int id)
        {
            var model = _adminRepository.GetAdmin(id);
            if(model != null)
            {
                AdminViewModel admin = new()
                {
                    AdminId = model.AdminId,
                    Email = model.Email,
                    Name = model.Name,
                    CreatedBy = model.CreatedBy,
                    CreatedTime = model.CreatedTime,
                    UpdatedBy = model.UpdatedBy,
                    UpdatedTime = model.UpdatedTime,
                };
                return admin;
            }
            return null;
        }

        public void AddAdmin(AdminViewModel model, string name)
        {
            var admin = new Admin();
            admin.Email = model.Email;
            admin.Password = PasswordManager.EncryptPassword(model.Password);
            admin.Name = model.Name;
            admin.CreatedBy = name;
            admin.CreatedTime = DateTime.Now;
            admin.UpdatedBy = name;
            admin.UpdatedTime = DateTime.Now;
            _adminRepository.AddAdmin(admin);
        }

        public bool CheckEmail(string email)
        {
            var isExist = _adminRepository.GetAdmins().Where(x => x.Email == email).Any();
            return isExist;
        }

        public void UpdateAdmin(AdminViewModel model, string name)
        {
            Admin admin = _adminRepository.GetAdmin(model.AdminId);
            if (admin != null)
            {
                admin.Email = model.Email;
                admin.Password = PasswordManager.EncryptPassword(model.Password);
                admin.Name = model.Name;
                admin.UpdatedBy= name;
                admin.UpdatedTime = DateTime.Now;
                _adminRepository.EditAdmin(admin);
            }
        }

        public void DeleteAdmin(AdminViewModel model)
        {
            Admin admin = _adminRepository.GetAdmin(model.AdminId);
            if (admin != null)
            {
                _adminRepository.DeleteAdmin(admin);
            }
        }
    }
}
