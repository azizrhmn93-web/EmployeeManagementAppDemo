using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModel
{
    public class CreateViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required]
       
        public Dept Departement { get; set; } 

        [Required(ErrorMessage = "Email is Required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public string Email {  get; set; }

        public IEnumerable<SelectListItem>? Departements { get; set; }

    }
}
