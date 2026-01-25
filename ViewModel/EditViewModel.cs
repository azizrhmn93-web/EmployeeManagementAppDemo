namespace EmployeeManagement.ViewModel
{
    public class EditViewModel : CreateViewModel
    {
        public int Id { get; set; }
        public string? existingPhotoPath { get; set; }
    }
}
