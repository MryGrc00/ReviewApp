using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using ASI.Basecode.Services.Interfaces;
using ASI.Basecode.Services.Manager;
using ASI.Basecode.Services.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        public AdminViewModel PaginatedAdmins(int page, int pageSize)
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

            int totalItems = data.Count;
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            page = Math.Max(1, Math.Min(page, totalPages));

            List<AdminViewModel> adminOnPage = data.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return new AdminViewModel
            {
                Admins = adminOnPage,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
            };
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
                    Password = model.Password,
                    CreatedBy = model.CreatedBy,
                    CreatedTime = model.CreatedTime,
                    UpdatedBy = model.UpdatedBy,
                    UpdatedTime = model.UpdatedTime,
                };
                return admin;
            }
            return null;
        }

        public AdminViewModel GetAdminWithPassword(int id)
        {
            var model = _adminRepository.GetAdmin(id);
            if (model != null)
            {
                AdminViewModel admin = new()
                {
                    AdminId = model.AdminId,
                    Email = model.Email,
                    Name = model.Name,
                    Password = PasswordManager.DecryptPassword(model.Password),
                    ConfirmPassword = PasswordManager.DecryptPassword(model.Password),
                    CreatedBy = model.CreatedBy,
                    CreatedTime = model.CreatedTime,
                    UpdatedBy = model.UpdatedBy,
                    UpdatedTime = model.UpdatedTime,
                };
                return admin;
            }
            return null;
        }

        public AdminViewModel GetAdminWithEmail(string email)
        {
            var model = _adminRepository.GetAdminByEmail(email);
            if(model != null)
            {
                AdminViewModel admin = new()
                {
                    AdminId = model.AdminId,
                    Email = model.Email,
                };
                return admin;
            }
            return null;
        }

        public AdminViewModel GetAdminWithName(string name)
        {
            var model = _adminRepository.GetAdminByName(name);
            if(model != null)
            {
                AdminViewModel admin = new()
                {
                    AdminId = model.AdminId,
                    Name = model.Name,
                    Email = model.Email,
                    Password = model.Password,
                };
                return admin;
            }
            return null;
        }

        public void AddAdmin(AdminViewModel model, string name)
        {
            var admin = new Admin();
            admin.Email = model.Email.ToLower();
            admin.Password = PasswordManager.EncryptPassword(model.Password);
            admin.Name = model.Name;
            admin.CreatedBy = name;
            admin.CreatedTime = DateTime.Now;
            admin.UpdatedBy = name;
            admin.UpdatedTime = DateTime.Now;
            _adminRepository.AddAdmin(admin);
        }

        public string CheckEmail(string email)
        {
            var isAdminExist = _adminRepository.GetAdmins().Any(x => x.Email == email);
            var isGmail = email.Contains("@gmail.com");

            if (isAdminExist)
            {
                return "Exist";
            }

            if (!isGmail)
            {
                return "Invalid";
            }

            return "Valid";
        }

        public string CheckOldPassword(int Adminid, string oldPassword)
        {
            var password = _adminRepository.GetAdmin(Adminid);

            if (password.Password == PasswordManager.EncryptPassword(oldPassword))
            {
                return "Match";
            }

            return "Not Match";
        }

        public string CheckPasswords(string password, string confirmPassword)
        {
            string pattern = @"^(?=.*[!@#$%^&*(),.?""\:{}|<>])(?=.*[a-z])(?=.*[A-Z])(?=.*\d).*$";

            if (!Regex.IsMatch(password, pattern))
            {
                return "PassInvalid";
            }

            if (password.Length < 8)
            {
                return "PassShort";
            }

            if (confirmPassword.Length < 8)
            {
                return "ConShort";
            }

            if (!Regex.IsMatch(confirmPassword, pattern))
            {
                return "ConInvalid";
            }

            if (password != confirmPassword)
            {
                return "NotMatch";
            }

            return "Passwords match and meet the criteria.";
        }

        public void UpdateAdmin(AdminViewModel model, string name)
        {
            Admin admin = _adminRepository.GetAdmin(model.AdminId);
            if (admin != null)
            {
                admin.AdminId = model.AdminId;
                admin.Email = model.Email.ToLower();
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

        public void ChangeEmail(AdminViewModel model, string name)
        {
            Admin admin = _adminRepository.GetAdmin(model.AdminId);
            if (admin != null)
            {
                admin.Email = model.Email.ToLower();
                admin.UpdatedBy = name;
                admin.UpdatedTime = DateTime.Now;
                _adminRepository.EditAdmin(admin);
            }
        }

        public void ChangeName(AdminViewModel model, string name)
        {
            Admin admin = _adminRepository.GetAdmin(model.AdminId);
            if (admin != null)
            {
                admin.Name = model.Name;
                admin.UpdatedBy = name;
                admin.UpdatedTime = DateTime.Now;
                _adminRepository.EditAdmin(admin);
            }
        }

        public void ChangePassword(AdminViewModel model, string name)
        {
            Admin admin = _adminRepository.GetAdmin(model.AdminId);
            if (admin != null)
            {
                admin.Password = PasswordManager.EncryptPassword(model.Password);
                admin.UpdatedBy = name;
                admin.UpdatedTime = DateTime.Now;
                _adminRepository.EditAdmin(admin);
            }
        }

    }
}
