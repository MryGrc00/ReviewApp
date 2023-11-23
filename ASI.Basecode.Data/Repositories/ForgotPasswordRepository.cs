using ASI.Basecode.Data.Interfaces;
using ASI.Basecode.Data.Models;
using Basecode.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASI.Basecode.Data.Repositories
{
    public class ForgotPasswordRepository : BaseRepository, IForgotPasswordRepository
    {
        public ForgotPasswordRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            
        }

        public ForgotPassword GetForgetPassword(string email)
        {
            var forgetPassword = this.GetDbSet<ForgotPassword>().FirstOrDefault(x => x.Email == email);
            return forgetPassword;
        }

        public void AddForgotPassword(ForgotPassword forgetPassword)
        {
            this.GetDbSet<ForgotPassword>().Add(forgetPassword);
            UnitOfWork.SaveChanges();
        }

        public void DeleteForgotPassword(ForgotPassword forgetPassword)
        {
            this.GetDbSet<ForgotPassword>().Remove(forgetPassword);
            UnitOfWork.SaveChanges();
        }
    }
}
