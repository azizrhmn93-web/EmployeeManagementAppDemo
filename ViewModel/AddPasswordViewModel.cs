using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModel
{
    public class AddPasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; } = string.Empty;
        [Required]
        [DataType (DataType.Password)]
        [Display(Name = "Confirm New Password")]
        public string ConfirmPassword { get; set;} = string.Empty;
    }
}
