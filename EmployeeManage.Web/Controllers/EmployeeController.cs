using EmployeeManage.Business.Interfaces;
using EmployeeManage.Business.Services;
using EmployeeManage.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManage.Web.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _service;
        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        // GET: /Employee
        public async Task<IActionResult> Index(string? name, string? title)
        {
            var list = await _service.GetEmployeesAsync(name, title);
            ViewData["NameFilter"] = name;
            ViewData["TitleFilter"] = title;
            return View(list);
        }

        // GET: /Employee/Add
        public IActionResult Add()
        {
            return View();
        }

        // POST: /Employee/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Employee employee, decimal salary, string title, DateTime fromDate)
        {
            if (!ModelState.IsValid)
            {
                return View(employee);
            }

            var salaryModel = new EmployeeSalary
            {
                FromDate = fromDate == default ? DateTime.UtcNow.Date : fromDate,
                ToDate = null,
                Title = title,
                Salary = salary
            };

            await _service.AddEmployeeAsync(employee, salaryModel);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EmployeeList(
    string name,
    string title,
    int page = 1,
    int pageSize = 10,
    string sortColumn = "Name",
    string sortOrder = "ASC")
        {
            var employees = await _service.GetEmployeesPagedAsync(name, title, page, pageSize, sortColumn, sortOrder);

            ViewBag.NameFilter = name;
            ViewBag.TitleFilter = title;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SortColumn = sortColumn;
            ViewBag.SortOrder = sortOrder;

            return View(employees);
        }

    }
}
