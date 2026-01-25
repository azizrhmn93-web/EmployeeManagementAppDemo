using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModel
{
    public class ProfileViewModel
    {
        public string? Id { get; set; } = null;
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; } = null;
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string? Email { get; set; } = null;
        [Display(Name = "Photo")]
        public string? ExistingPhotoPath { get; set; } = null;
        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Full Name is required")]
        public string? FullName { get; set; } = null;
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; } = null;

        public IFormFile? Photo { get; set; } = null;


    }
}
