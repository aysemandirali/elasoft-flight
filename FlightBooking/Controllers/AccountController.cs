using System.Security.Claims;
using FlightBooking.Services.AccountServices;
using FlightBooking.Services.BookingServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IBookingService _bookingService;

        public AccountController(IAuthService authService, IBookingService bookingService)
        {
            _authService = authService;
            _bookingService = bookingService;
        }

        // Müşteri hesap sayfası (Hesabım): kendi rezervasyonlarını gösterir
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            var all = await _bookingService.GetAllBookingsAsync();
            var myBookings = all.Where(b => b.ContactEmail.Equals(email, StringComparison.OrdinalIgnoreCase)).ToList();
            ViewBag.FullName = User.Identity?.Name;
            ViewBag.Email = email;
            return View(myBookings);
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string fullName, string email, string password)
        {
            var ok = await _authService.RegisterAsync(fullName, email, password);
            if (!ok)
            {
                ViewBag.Error = "Bu e-posta ile zaten bir kayıt var.";
                return View();
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _authService.ValidateLoginAsync(email, password);
            if (user == null)
            {
                ViewBag.Error = "E-posta veya şifre hatalı.";
                return View();
            }

            // Cerez tabanli oturum ac (rol dahil)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            // Role gore yonlendir: admin -> panel, musteri -> Hesabim
            if (user.Role == "Admin")
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            return RedirectToAction("Profile");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Default");
        }
    }
}
