using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities.ApplicationUsers;

namespace Application.Interface.User
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserInfoByUserID(string UserID);
    }
}
