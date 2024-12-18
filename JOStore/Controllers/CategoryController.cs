using Microsoft.AspNetCore.Mvc;

namespace JOStore.Controllers
{
    public class CategoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
