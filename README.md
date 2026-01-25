# EmployeeManagementAppDemo

A demo **ASP.NET Core MVC** application for managing employees, including authentication, administration features, and Identity integration.

---

## Table of Contents

* [Project Overview](#project-overview)
* [Features](#features)
* [Project Structure](#project-structure)
* [Changelog / Updates](#changelog--updates)
* [Setup & Installation](#setup--installation)
* [Usage](#usage)
* [Contributing](#contributing)
* [License](#license)

---

## Project Overview

This project demonstrates a simple yet robust **Employee Management System** built with **ASP.NET Core MVC**.
It integrates:

* User authentication and authorization via **ASP.NET Identity**
* Admin functionality for managing employees
* Error handling and logging
* Database migrations to manage schema changes

It’s designed as a demo to showcase core functionality and best practices in ASP.NET MVC application development.

---

## Features

* User registration, login, and logout
* Admin panel for managing users and employees
* Custom error pages for better user experience
* Identity integration with extended user properties
* Database migrations for schema management

---

## Project Structure

* `Controllers/` – Contains MVC controllers for handling HTTP requests:

  * `AccountController.cs` – User account operations
  * `AdministrationController.cs` – Admin-specific operations
  * `ErrorController.cs` – Error handling
  * `HomeController.cs` – Main landing page logic

* `Migrations/` – Database migration scripts for Identity and extended user schema

* `EmployeeManagement.csproj` – Project file with references and configurations

* `EmployeeManagement.sln` – Visual Studio solution file

* `.gitignore` – Files/folders ignored by Git

* `.gitattributes` – Git attributes for consistent line endings and encoding

---

## Changelog / Updates

The following updates have been made in the **master branch** compared to **main**:

### Added

* `.gitattributes` – Git configuration for line endings
* `Controllers/AccountController.cs` – Handles user account operations
* `Controllers/AdministrationController.cs` – Admin-specific functionality
* `Controllers/ErrorController.cs` – Error handling
* `Migrations/20251230093701_CreateIdentitySchema.*` – Initial Identity schema migration
* `Migrations/20260102034821_Extend_IdentityUser.Designer.cs` – Extending Identity user

### Modified

* `.gitignore` – Updated ignored files/folders
* `Controllers/HomeController.cs` – Updated home page logic
* `EmployeeManagement.csproj` – Project updated for new controllers/migrations
* `EmployeeManagement.sln` – Solution file updated

---

## Setup & Installation

1. **Clone the repository:**

```bash
git clone https://github.com/azizrhmn93-web/EmployeeManagementAppDemo.git
cd EmployeeManagementAppDemo
```

2. **Open the solution** in Visual Studio 2022 (or newer).

3. **Restore NuGet packages** (Visual Studio usually does this automatically).

4. **Apply database migrations:**

```powershell
Update-Database
```

5. **Run the application:**

* Press `F5` in Visual Studio
* Or use `dotnet run` from the terminal in the project directory

---

## Usage

* Register a new user via the registration page
* Log in with your credentials
* Access admin functionalities (if your account has admin roles)
* Navigate between Home, Admin, and Error pages to explore functionality

---

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -m "Add feature"`)
4. Push to the branch (`git push origin feature/YourFeature`)
5. Open a Pull Request

---
