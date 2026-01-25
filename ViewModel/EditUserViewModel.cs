using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using EmployeeManagement.Models;

namespace EmployeeManagement.ViewModel
{
    // Keep as a view model (inherits AppUser) but redeclare the properties with validation attributes.
    public class EditUserViewModel : AppUser
    {
        [Required]
        [Display(Name = "User name*")]
        public new string UserName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Full name*")]
        public new string FullName { get; set; } = string.Empty;

        public new IList<string>? UserRoles { get; set; } 

        public new List<string>? Claims { get; set; }
    }
}