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
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork) 
        {

        }

        public IQueryable<Admin> GetUsers()
        {
            return this.GetDbSet<Admin>();
        }

        public bool UserExists(string userId)
        {
            return this.GetDbSet<Admin>().Any(x => x.Email == userId);
        }

        public void AddUser(Admin admin)
        {
            this.GetDbSet<Admin>().Add(admin);
            UnitOfWork.SaveChanges();
        }

    }
}
