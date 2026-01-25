using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModel
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Username or Email")]
        public string Input { get; set; } = string.Empty;
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }
    }
}
