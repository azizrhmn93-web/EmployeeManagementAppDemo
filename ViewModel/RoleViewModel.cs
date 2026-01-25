using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModel
{
    public class RoleViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public  string RoleName { get; set; } = string.Empty;
    }
}
