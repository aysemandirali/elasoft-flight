using System.Security.Claims;
using FlightBooking.Services.AccountServices;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.FlightServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ProfileController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;

        public ProfileController(IAuthService authService, IFlightService flightService, IBookingService bookingService)
        {
            _authService = authService;
            _flightService = flightService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";
            var user = await _authService.GetByEmailAsync(email);

            ViewBag.FullName = user?.FullName ?? User.Identity?.Name;
            ViewBag.Email = user?.Email ?? email;
            ViewBag.Role = user?.Role ?? "Admin";

            // Yöneticinin sistemdeki etkisini gösteren gerçek sayılar
            var flights = await _flightService.GetAllFlightsAsync();
            var bookings = await _bookingService.GetAllBookingsAsync();
            ViewBag.FlightCount = flights.Count;
            ViewBag.BookingCount = bookings.Count;
            ViewBag.Revenue = bookings.Sum(b => b.TotalPrice);

            return View();
        }
    }
}
