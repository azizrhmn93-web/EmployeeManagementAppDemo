namespace EmployeeManagement.ViewModel
{
    public class EditUserClaimsViewModel
    {
        public EditUserClaimsViewModel()
        {
            Claims = new List<UserClaimViewModel>();
        }
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public List<UserClaimViewModel>? Claims { get; set; }
    }
}
