using AspNetCoreGeneratedDocument;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModel;
using EmployeeManagement.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Controllers
{
    //Attribute routing to Home 
    //[Route("")]
    //[Route("[controller]")]
    public class HomeController : Controller
    {
        private IEmployeeDepository _employeeDepository;
        public HomeController(IEmployeeDepository employeeDepository)
        {
            _employeeDepository = employeeDepository;
        }

        //[Route("")]
        //[Route("[action]")]
        public IActionResult Index()
        {
            var model = _employeeDepository.Employees();
            return View(model);
        }

        //[Route("[action]/{id}")]
        [HttpGet]
        public IActionResult Details(int? id)
        {
            DetailViewModel viewModel = new DetailViewModel()
            {
                Employee = _employeeDepository.GetEmployee(id??1),
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

            if (!ModelState.IsValid)
            {
                model.Departements = EnumExtension.ToSelectList<Dept>();
                return View(model);
            }

            //binding data to Employee object
            var employee = new Employee()
            { 
                Name = model.Name,
                Departement = model.Departement,
                Email = model.Email,
            };
            

            _employeeDepository.AddEmployee(employee);
            return RedirectToAction("Index");

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
