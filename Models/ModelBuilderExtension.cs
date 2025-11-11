using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models
{
    public static class ModelBuilderExtension
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>().HasData(
                new Employee() { Id = 1, Departement = Dept.Payroll, Name = "Aziz", Email = "aziz.rhmn93@gmail.com", PhotoPath="" },
                new Employee() { Id = 2, Departement = Dept.Payroll, Name = "Jhon", Email = "jhon3@gmail.com", PhotoPath= "" }
                );
        }
    }
}
