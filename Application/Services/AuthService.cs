using Application.Interface.Auth;
using Domain.Entities.ApplicationUsers;
using Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Application.Validation;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<bool> RegisterAsync(RegisterDto dto)
        {
            // Email format validation
            if (!UserValidation.IsValidEmail(dto.Email))
            {
                throw new ArgumentException("Invalid email format. Please provide a valid email address.");
            }

            // Password strength validation
            var passwordValidationResult = UserValidation.ValidatePassword(dto.Password);
            if (!passwordValidationResult.IsValid)
            {
                throw new ArgumentException($"Password validation failed: {passwordValidationResult.ErrorMessage}");
            }

            // Check for duplicate emails prior to account creation
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("An account with this email address already exists. Please use a different email or try logging in.");
            }

            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"User registration failed: {errors}");
            }

            return result.Succeeded;
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, dto.Password))
            {
                return GenerateJwtToken(user);
            }
            return null;
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
