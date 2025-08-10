using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.ApplicationUsers;

namespace Application.Interface.User
{
    public interface IUserRepository
    {
        Task<ApplicationUser> GetApplicationUserById(Guid id);
    }
}
