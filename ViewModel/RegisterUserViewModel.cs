using EmployeeManagement.Utilities;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModel
{
    public class RegisterUserViewModel
    {
        [Required]
        [EmailAddress]
        [Remote(action: "ValidateEmail", controller: "Account")]
        //[EmailDomain(allowedDomain: "mycompany.com", ErrorMessage = "Email domain must be mycompany.com")]
        public string Email { get; set; } = string.Empty;

      [Display(Name = "Username")]
        public string? Username { get; set; }   = null;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } =  string.Empty;
    }
}
