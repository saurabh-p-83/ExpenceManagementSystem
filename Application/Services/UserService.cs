using Application.DTOs;
using Application.Interface.User;
using AutoMapper;
using Domain.Entities.ApplicationUsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<UserProfileDto> GetUserInfoByUserID(string UserID)
        {
            if (!Guid.TryParse(UserID, out var guid))
            {
                throw new ArgumentException("Invalid user ID format");
            }

            var DbResponse = await _userRepository.GetApplicationUserById(guid);
            if (DbResponse == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            var user = _mapper.Map<UserProfileDto>(DbResponse);
            
            return user;
        }
    }
}

