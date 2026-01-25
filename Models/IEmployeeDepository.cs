namespace EmployeeManagement.Models
{
    public interface IEmployeeDepository
    {
        public Employee GetEmployee(int  id);
        public IEnumerable<Employee> Employees();
        public Employee AddEmployee(Employee employee);
        public Employee UpdateEmployee(Employee employeeUpdates);
        public Employee DeleteEmployee(int id);
    }
}
