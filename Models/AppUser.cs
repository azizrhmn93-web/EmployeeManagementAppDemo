using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.FileProviders;

namespace EmployeeManagement.Models
{
    public class AppUser : IdentityUser
    {
        // Additional properties can be added here in the future
        public  string FullName { get; set; } = string.Empty;
        public string? ProfilePicture { get; set; } = null;
    }
}
