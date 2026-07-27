using FlightBooking.Services.BookingServices;
using FlightBooking.Services.FlightServices;
using FlightBooking.Services.NoShowServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;
        private readonly INoShowService _noShowService;

        public DashboardController(IFlightService flightService, IBookingService bookingService, INoShowService noShowService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
            _noShowService = noShowService;
        }

        public async Task<IActionResult> Index()
        {
            var flights = await _flightService.GetAllFlightsAsync();
            var bookings = await _bookingService.GetAllBookingsAsync();
            var noshow = await _noShowService.GetAllAsync();

            ViewBag.FlightCount = flights.Count;
            ViewBag.BookingCount = bookings.Count;
            ViewBag.NoShowCount = noshow.Count;
            ViewBag.PassengerTotal = bookings.Sum(b => b.PassengerCount);
            ViewBag.Scheduled = flights.Count(f => f.Status == "Scheduled");
            ViewBag.Delayed = flights.Count(f => f.Status == "Delayed");
            ViewBag.Revenue = bookings.Sum(b => b.TotalPrice);

            return View();
        }
    }
}
