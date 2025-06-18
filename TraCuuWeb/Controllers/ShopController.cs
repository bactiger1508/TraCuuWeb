using Microsoft.AspNetCore.Mvc;

namespace TraCuuWeb.Controllers
{
    public class ShopController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
} 