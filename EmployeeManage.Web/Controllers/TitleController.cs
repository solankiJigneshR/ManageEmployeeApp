using EmployeeManage.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManage.Web.Controllers
{
    public class TitleController : Controller
    {
        private readonly IEmployeeService _service;
        public TitleController(IEmployeeService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var list = await _service.GetTitlesAsync();
            return View(list);
        }
    }
}
