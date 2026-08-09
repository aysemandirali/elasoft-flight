using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    // Footer'daki kurumsal bilgi sayfaları (Kullanım Koşulları, Gizlilik, Çerez, Site Haritası).
    public class InfoController : Controller
    {
        public IActionResult About() => View();
        public IActionResult Terms() => View();
        public IActionResult Privacy() => View();
        public IActionResult Cookies() => View();
        public IActionResult SiteMap() => View();
    }
}
