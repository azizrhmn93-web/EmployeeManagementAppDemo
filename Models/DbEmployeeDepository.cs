
namespace EmployeeManagement.Models
{
    public class DbEmployeeDepository : IEmployeeDepository
    {
        private readonly AppDBContext _context;

        public DbEmployeeDepository(AppDBContext context)
        {
            _context = context;
        }

        public Employee AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee); 
            _context.SaveChanges();
            return employee;
        }

        public Employee DeleteEmployee(int id)
        {
            var emp = _context.Employees.Find(id);
            if (emp != null)
            {
                _context.Employees.Remove(emp);
            }
            _context.SaveChanges();
            return emp;
        }

        public IEnumerable<Employee> Employees()
        {
            return _context.Employees;
        }

        public Employee GetEmployee(int id)
        {
            return _context.Employees.Find(id);
            
        }

        public Employee UpdateEmployee(Employee employeeUpdates)
        {
            var emp = _context.Employees.Attach(employeeUpdates);
            emp.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
            return employeeUpdates;

        }
    }
}
