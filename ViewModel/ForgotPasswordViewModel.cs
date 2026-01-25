using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModel
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
