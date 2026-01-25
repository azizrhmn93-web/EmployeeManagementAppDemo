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
    //Attribute routing to Home 
    //[Route("")]
    //[Route("[controller]")]

    public class HomeController : Controller
    {
        private IEmployeeDepository _employeeDepository;
        private readonly IWebHostEnvironment hostEnvironment;

        public HomeController(IEmployeeDepository employeeDepository, IWebHostEnvironment hostEnvironment)
        {
            _employeeDepository = employeeDepository;
            this.hostEnvironment = hostEnvironment;
        }

        //[Route("")]
        //[Route("[action]")]
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

                ViewBag.SearchQuery = term;
            }

            return View(model);
        }

        //[Route("[action]/{id}")]
        [HttpGet]
        public IActionResult Details(int id)
        {
            //throw new Exception("Error in Details View");
            Employee employee = _employeeDepository.GetEmployee(id);
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
                string uniqueName = ProcessUploadFile(model);

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

        private string ProcessUploadFile(CreateViewModel model)
        {
            string uniqueName = null;
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
            Employee employee = _employeeDepository.GetEmployee(id);
            EditViewModel editViewModel = new EditViewModel
            {
                Id = employee.Id,
                Name = employee.Name,
                Departement = employee.Departement,
                Email = employee.Email,
                existingPhotoPath = employee.PhotoPath,
                Departements = EnumExtension.ToSelectList<Dept>(),
                DateOfBirth = employee.DateOfBirth
            };
            return View(editViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Employee employee = _employeeDepository.GetEmployee(model.Id);
                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.Departement = model.Departement;
                employee.DateOfBirth = model.DateOfBirth;
                if (model.existingPhotoPath != null)
                {
                    if (model.Photo != null)
                    {
                        string filePath = Path.Combine(hostEnvironment.WebRootPath, "images", model.existingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                }
                if (model.Photo == null)
                    employee.PhotoPath = model.existingPhotoPath;
                else
                    employee.PhotoPath = ProcessUploadFile(model);
                _employeeDepository.UpdateEmployee(employee);
                return RedirectToAction("index");
            }
            model.Departements = EnumExtension.ToSelectList<Dept>();
            return View();
        }

        [HttpPost]
        public IActionResult DeleteEmployee(int id)
        {
            Employee employee = _employeeDepository.GetEmployee(id);
            if(employee == null)
            {
                ViewBag.Message = $"Employee with {id} cannot be found";
                return RedirectToAction("NotFound");
            }

            _employeeDepository.DeleteEmployee(id);
            TempData["ToastMessage"] = "Employee deleted successfully.";
            return RedirectToAction("index");

        }
        // Passing data to a view throug ViewBag property
        //attribute routing
        //[Route("[action]/{id:int:min(1):max(3)}")]
        //public IActionResult Details(int id)
        //{
        //    var model = _employeeDepository.Get(id);
        //    ViewBag.PageTitle = "Employee Details";
        //    ViewBag.EmployeeDetail = model;
        //    return View("DetailsByViewBag", model);
        //}

    }
}
