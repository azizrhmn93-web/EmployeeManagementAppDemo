// Delete Confirmation Modal Logic
// This script handles the dynamic population of the delete confirmation modal
// assigning the Id and Name of the item to be deleted.
document.addEventListener('DOMContentLoaded', function () {
    var deleteModal = document.getElementById('confirmDeleteModal');

    if (deleteModal) {
        deleteModal.addEventListener('show.bs.modal', function (event) {
            // 1. The button that was clicked
            var button = event.relatedTarget;

            // 2. Extract info from the data-bs-* attributes we set in the loop
            var Id = button.getAttribute('data-bs-id');
            var Name = button.getAttribute('data-bs-name');

            // 3. Update the Modal's text and hidden inputs
            // This finds the <strong> tag where the name is displayed
            var modalNamePlaceholder = deleteModal.querySelector('.modal-body strong');
            // This finds the hidden input for the ID
            var modalIdInput = deleteModal.querySelector('input[name="id"]');
            var modalUserIdInput = deleteModal.querySelector('input[name="userId"]');

            if (modalNamePlaceholder) {
                modalNamePlaceholder.textContent = Name;
            }
            if (modalIdInput) {
                modalIdInput.value = Id;
            }
            if (modalUserIdInput) {
                modalUserIdInput.value = Id;
            }
        });
    }
});