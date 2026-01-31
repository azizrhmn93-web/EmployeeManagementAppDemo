using AspNetCoreGeneratedDocument;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModel;
using EmployeeManagement.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;


namespace EmployeeManagement.Controllers
{
    public class HomeController : Controller
    {
        private IEmployeeDepository _employeeDepository;
        private readonly IWebHostEnvironment hostEnvironment;

        //Constructor Injection
        public HomeController(IEmployeeDepository employeeDepository, IWebHostEnvironment hostEnvironment)
        {
            _employeeDepository = employeeDepository;
            this.hostEnvironment = hostEnvironment;
        }


        [AllowAnonymous]
        public IActionResult Index(string? q)
        {
            var model = _employeeDepository.Employees() ?? Enumerable.Empty<Employee>();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var term = q.Trim();

                // Filter by name, email or department (case-insensitive)
                model = model.Where(e =>
                    (!string.IsNullOrEmpty(e.Name) && e.Name.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(e.Email) && e.Email.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                    e.Departement.ToString().Contains(term, StringComparison.OrdinalIgnoreCase)
                );
                // Pass the search term to the view for display 
                ViewBag.SearchQuery = term;
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult Details(int id)
        {
            //throw new Exception("Error in Details View");
            Employee? employee = _employeeDepository.GetEmployee(id);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }

            DetailViewModel viewModel = new DetailViewModel()
            {
                Employee = employee,
                PageTitle = "Employee Details"
            };

            return View(viewModel);
        }

        //[Route("[action]")]
        [HttpGet]
        public IActionResult Create()
        {
            CreateViewModel viewModel = new CreateViewModel();
            viewModel.Departements = EnumExtension.ToSelectList<Dept>();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Create(CreateViewModel model)
        {

            if (ModelState.IsValid)
            {
                string? uniqueName = ProcessUploadFile(model);

                //binding data to Employee object
                Employee newEmployee = new Employee()
                {
                    Name = model.Name,
                    Departement = model.Departement,
                    Email = model.Email,
                    PhotoPath = uniqueName,
                    DateOfBirth = model.DateOfBirth
                };

                _employeeDepository.AddEmployee(newEmployee);
                return RedirectToAction("details", new { id = newEmployee.Id });
            }

            model.Departements = EnumExtension.ToSelectList<Dept>();
            return View(model);
        }

        private string? ProcessUploadFile(CreateViewModel model)
        {
            string? uniqueName = null;
            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostEnvironment.WebRootPath, "images");
                uniqueName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    model.Photo.CopyTo(fileStream);
            }

            return uniqueName;
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Employee? employee = _employeeDepository.GetEmployee(id);
            if (employee == null)
            {
                Response.StatusCode = 404;
                return View("EmployeeNotFound", id);
            }
            EditViewModel editViewModel = new EditViewModel()
            {
                Id = employee.Id,
                Name = employee.Name,
                Email = employee.Email,
                Departement = employee.Departement,
                ExistingImagePath = employee.PhotoPath,
                DateOfBirth = employee.DateOfBirth
            };
            editViewModel.Departements = EnumExtension.ToSelectList<Dept>();
            return View(editViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee? employee = _employeeDepository.GetEmployee(model.Id);
                if (employee == null)
                {
                    Response.StatusCode = 404;
                    return View("EmployeeNotFound", model.Id);
                }
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Departement = model.Departement;
                employee.DateOfBirth = model.DateOfBirth;
                if (model.Photo != null)
                {
                    DeleteFile(model.ExistingImagePath);
                    employee.PhotoPath = ProcessUploadFile(model);
                }
                else if (model.IsImageRemoved)
                {
                    DeleteFile(model.ExistingImagePath);
                    employee.PhotoPath = null;
                }
                _employeeDepository.UpdateEmployee(employee);
                return RedirectToAction("index");
            }
            model.Departements = EnumExtension.ToSelectList<Dept>();
            return View(model);


            // Local function to delete a file
            void DeleteFile(string? FileName)
            {
                if (!string.IsNullOrEmpty(FileName))
                {
                    string filePath = Path.Combine(hostEnvironment.WebRootPath, "images", FileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }
        }

        [HttpPost]
        public IActionResult DeleteEmployee(int id)
        {
            Employee? employee = _employeeDepository.GetEmployee(id);
            if(employee == null)
            {
                ViewBag.Message = $"Employee with {id} cannot be found";
                return RedirectToAction("NotFound");
            }

            _employeeDepository.DeleteEmployee(id);
            TempData["ToastMessage"] = "Employee deleted successfully.";
            return RedirectToAction("index");

        }
    }
}
