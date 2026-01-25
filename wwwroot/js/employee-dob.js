// JavaScript for Employee Date of Birth and Photo Input
document.addEventListener("DOMContentLoaded", function () {
    const today = new Date();
    const maxYear = today.getFullYear() - 18;
    const minYear = today.getFullYear() - 60;

    const dobInput = document.getElementById("DateOfBirth");
    if (!dobInput) return; // do nothing if input doesn't exist

    const defaultDate = dobInput.value ? new Date(dobInput.value) : null;

    flatpickr(dobInput, {
        dateFormat: "Y-m-d",
        maxDate: new Date(maxYear, today.getMonth(), today.getDate()),
        minDate: new Date(minYear, today.getMonth(), today.getDate()),
        disableMobile: true,
        allowInput: false,
        defaultDate: defaultDate
    });

    // Photo input logic
    const photoText = document.getElementById('photoText');
    const photoInput = document.getElementById('photoInput');
    if (photoText && photoInput) {
        photoText.addEventListener('click', () => photoInput.click());
        photoInput.addEventListener('change', () => {
            photoText.textContent = photoInput.files[0]?.name || 'Choose Photo';
        });
    }
});
