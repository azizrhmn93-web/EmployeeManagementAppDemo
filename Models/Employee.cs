using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public  string Name { get; set; }

        public string Email { get; set; }

        public Dept Departement { get; set; }

        public string? PhotoPath { get; set; }
    }
}
