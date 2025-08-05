using Microsoft.AspNetCore.Mvc;

namespace Frontendd.Controllers
{
    public class SchoolController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult UserManage()
        {
            return View();
        }
        public IActionResult ClassManage()
        {
            return View();
        }
        public IActionResult SubjectManage()
        {
            return View();
        }
        public IActionResult ScheduleManage()
        {
            return View();
        }
    }
}
