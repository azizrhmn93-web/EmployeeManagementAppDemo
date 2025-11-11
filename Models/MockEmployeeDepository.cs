
namespace EmployeeManagement.Models
{
    public class MockEmployeeDepository : IEmployeeDepository
    {
        List<Employee> employeeList;
        public MockEmployeeDepository()
        {
            employeeList = new List<Employee>()
            {
                new Employee() {Id = 1, Name = "Aziz", Email = "aziz.rhmn93@gmail.com", Departement = Dept.Development},
                new Employee() {Id = 2, Name = "Jhon", Email = "Jhon_doe@gmail.com", Departement = Dept.Payroll},
                new Employee() {Id = 3, Name = "Doe", Email = "doe@gmail.com", Departement = Dept.HR},
                new Employee() {Id = 4, Name = "Muller", Email = "muller@gmail.com", Departement = Dept.Development}
            };
        }

        public IEnumerable<Employee> Employees()
        {
            return employeeList;
        }

        public Employee GetEmployee(int id)
        {
            return employeeList.FirstOrDefault(e => e.Id == id);
        }

        public Employee AddEmployee(Employee employee)
        { 
            employee.Id = employeeList.Max(e => e.Id) + 1;
            employeeList.Add(employee); 
            return employee;
        }

        public Employee UpdateEmployee(Employee employeeUpdates)
        {
            var emp = employeeList.FirstOrDefault(e => e.Id == employeeUpdates.Id);
            if (emp != null)
            {
                emp.Name = employeeUpdates.Name;
                emp.Email = employeeUpdates.Email;
                emp.Departement = employeeUpdates.Departement;
            }
            return emp;
            
        }

        public Employee DeleteEmployee(int id)
        {
            var emp = employeeList.FirstOrDefault(e => e.Id ==id);
            if (emp != null)
            {
                employeeList.Remove(emp);
            }
            return emp;
        }
    }
}
