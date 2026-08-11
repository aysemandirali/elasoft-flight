using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // Beklenmedik hata ve bulunamayan sayfalar icin ortak ekran.
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public IActionResult Error(int? code)
        {
            ViewBag.Code = code;
            return View();
        }
    }
}
