using FlightBooking.Dtos.BookingDtos;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.FlightServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    // Halka acik (musteri) ucus arama ve bilet alma.
    public class FlightController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;

        public FlightController(IFlightService flightService, IBookingService bookingService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
        }

        // Ucus listesi + basit arama (kalkis/varis koduna gore)
        [HttpGet]
        public async Task<IActionResult> Index(string? from, string? to)
        {
            var flights = await _flightService.GetAllFlightsAsync();

            // Sadece bos koltugu olan ucuslar
            flights = flights.Where(x => x.AvailableSeats > 0).ToList();

            if (!string.IsNullOrWhiteSpace(from))
                flights = flights.Where(x => x.DepartureAirportCode.Contains(from.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(to))
                flights = flights.Where(x => x.ArrivalAirportCode.Contains(to.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.From = from;
            ViewBag.To = to;
            return View(flights);
        }

        // Secilen ucus icin bilet alma formu
        [HttpGet]
        public async Task<IActionResult> Book(string id)
        {
            var flight = await _flightService.GetFlightByIdAsync(id);
            return View(flight);
        }

        // Bilet alma formunu kaydet, PNR ile onay sayfasina gec
        [HttpPost]
        public async Task<IActionResult> Book(CreateBookingDto dto)
        {
            var pnr = await _bookingService.CreateBookingAsync(dto);
            TempData["Pnr"] = pnr;
            return RedirectToAction("Confirmation");
        }

        // Onay sayfasi (PNR gosterir)
        [HttpGet]
        public IActionResult Confirmation()
        {
            ViewBag.Pnr = TempData["Pnr"] as string;
            return View();
        }
    }
}
