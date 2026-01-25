using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.ViewModel
{
    public class EditRoleViewModel : RoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();
        }
        public string Id { get; set; }

        public List<string>? Users { get; set; }
    }
}
