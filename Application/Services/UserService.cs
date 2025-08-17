using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interface.User;
using Domain.Entities.ApplicationUsers;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        { 
            _userRepository = userRepository;
        }
        public async Task<ApplicationUser> GetUserInfoByUserID(string UserID)
        {
            if (!Guid.TryParse(UserID, out var guid))
            {
                throw new ArgumentException("Invalid user ID format");
            }

            var user = await _userRepository.GetApplicationUserById(guid);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return user;
        }
    }
}

