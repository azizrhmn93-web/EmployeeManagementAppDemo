using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModel
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [Required]
        [Compare("Password", ErrorMessage = "Password and confirm password don't match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
