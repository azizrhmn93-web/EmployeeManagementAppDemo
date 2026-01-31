namespace EmployeeManagement.ViewModel
{
    public class EditViewModel : CreateViewModel
    {
        public int Id { get; set; }
        public string? ExistingImagePath { get; set; }
        public bool IsImageRemoved { get; set; }
    }
}
