document.addEventListener("DOMContentLoaded", function () {
    // Select ALL elements with the class
    const togglers = document.querySelectorAll(".toggle-password");

    togglers.forEach(btn => {
        btn.addEventListener("click", function () {
            // 1. Find the input that is in the same container as this specific button
            const container = this.closest(".position-relative");
            const input = container.querySelector(".password-input");
            const icon = this.querySelector("i");

            // 2. Toggle the type
            const isPassword = input.getAttribute("type") === "password";
            input.setAttribute("type", isPassword ? "text" : "password");

            // 3. Toggle the icon classes
            icon.classList.toggle("bi-eye-fill");
            icon.classList.toggle("bi-eye-slash-fill");
        });
    });
});