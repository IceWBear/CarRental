using Microsoft.AspNetCore.Mvc;

namespace Project_RentACar.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return View("Test");
        }
    }
}
