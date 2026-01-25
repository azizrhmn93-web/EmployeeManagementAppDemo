using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModel
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Current Password")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = string.Empty;


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = null;


        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "New Password and Confirm New Password do not match.")]
        public string ConfirmNewPassword { get; set; } = null;
    }
}
