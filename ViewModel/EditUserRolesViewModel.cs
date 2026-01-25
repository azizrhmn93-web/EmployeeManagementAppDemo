namespace EmployeeManagement.ViewModel
{
    public class EditUserRolesViewModel
    {
        public string userName {  get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public string roleId { get; set; } = string.Empty;
        public string roleName { get; set; } = string.Empty;
        public bool isSelected { get; set; }
    }
}
