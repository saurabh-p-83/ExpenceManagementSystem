using Application.DTOs;
using Domain.Entities.ApplicationUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interface.User
{
    public interface IUserService
    {
        Task<UserProfileDto> GetUserInfoByUserID(string UserID);
    }
}
