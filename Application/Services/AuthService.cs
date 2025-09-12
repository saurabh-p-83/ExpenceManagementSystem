using Application.Interface.Auth;
using Domain.Entities.ApplicationUsers;
using Domain.Entities.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
            if (!IsValidEmail(dto.Email))
            {
                throw new ArgumentException("Invalid email format. Please provide a valid email address.");
            }

            // Password strength validation
            var passwordValidationResult = ValidatePassword(dto.Password);
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

        /// <summary>
        /// Validates email format using regex pattern
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>True if email format is valid</returns>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use a more comprehensive email regex pattern
                var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
                return Regex.IsMatch(email, emailPattern, RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates password strength requirements
        /// </summary>
        /// <param name="password">Password to validate</param>
        /// <returns>Validation result with success status and error message</returns>
        private PasswordValidationResult ValidatePassword(string password)
        {
            var result = new PasswordValidationResult { IsValid = true };
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                return new PasswordValidationResult 
                { 
                    IsValid = false, 
                    ErrorMessage = "Password is required." 
                };
            }

            // Minimum length requirement
            if (password.Length < 8)
            {
                errors.Add("Password must be at least 8 characters long.");
            }

            // Maximum length requirement (prevent DoS attacks)
            if (password.Length > 128)
            {
                errors.Add("Password must not exceed 128 characters.");
            }

            // At least one uppercase letter
            if (!password.Any(char.IsUpper))
            {
                errors.Add("Password must contain at least one uppercase letter.");
            }

            // At least one lowercase letter
            if (!password.Any(char.IsLower))
            {
                errors.Add("Password must contain at least one lowercase letter.");
            }

            // At least one digit
            if (!password.Any(char.IsDigit))
            {
                errors.Add("Password must contain at least one number.");
            }

            // At least one special character
            var specialChars = @"!@#$%^&*()_+-=[]{}|;:,.<>?";
            if (!password.Any(c => specialChars.Contains(c)))
            {
                errors.Add("Password must contain at least one special character (!@#$%^&*()_+-=[]{}|;:,.<>?).");
            }

            // Check for common weak patterns
            var commonPatterns = new[] { "123456", "password", "qwerty", "abc123", "111111" };
            if (commonPatterns.Any(pattern => password.ToLower().Contains(pattern)))
            {
                errors.Add("Password contains common weak patterns. Please choose a more secure password.");
            }

            if (errors.Any())
            {
                result.IsValid = false;
                result.ErrorMessage = string.Join(" ", errors);
            }

            return result;
        }

        /// <summary>
        /// Password validation result container
        /// </summary>
        private class PasswordValidationResult
        {
            public bool IsValid { get; set; }
            public string ErrorMessage { get; set; } = string.Empty;
        }
    }
}
