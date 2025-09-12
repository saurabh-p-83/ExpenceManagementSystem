using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Application.Validation
{
    /// <summary>
    /// Provides user input validation functionality
    /// </summary>
    public static class UserValidation
    {
        /// <summary>
        /// Validates email format using regex pattern
        /// </summary>
        /// <param name="email">Email to validate</param>
        /// <returns>True if email format is valid</returns>
        public static bool IsValidEmail(string email)
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
        public static PasswordValidationResult ValidatePassword(string password)
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
    }

    /// <summary>
    /// Password validation result container
    /// </summary>
    public class PasswordValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
