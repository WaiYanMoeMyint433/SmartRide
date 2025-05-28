using Microsoft.AspNetCore.Mvc;

namespace SmartRide.Controllers
{
    public class TestController : Controller
    {
        public IActionResult Index()
        {
            return Content("<h1>TEST CONTROLLER WORKS!</h1><a href='/Account/Login'>Go to Login</a>", "text/html");
        }
    }
}