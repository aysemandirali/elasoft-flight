using FlightBooking.Dtos.BookingDtos;
using FlightBooking.Entities;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.CheckInServices;
using FlightBooking.Services.FlightServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    // Halka acik (musteri) ucus arama, bilet alma, seyahat sorgulama ve check-in.
    public class FlightController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;
        private readonly ICheckInService _checkInService;

        public FlightController(IFlightService flightService, IBookingService bookingService, ICheckInService checkInService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
            _checkInService = checkInService;
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
        // Rezervasyon sorgulama (Seyahatlerim): PNR + soyad ile rezervasyonu göster
        [HttpGet]
        public async Task<IActionResult> MyBooking(string? pnr, string? surname)
        {
            ViewBag.Searched = !string.IsNullOrWhiteSpace(pnr);
            ViewBag.Pnr = pnr;
            ViewBag.Surname = surname;

            if (string.IsNullOrWhiteSpace(pnr))
                return View((Booking?)null);

            var booking = await _bookingService.GetByPnrAsync(pnr.Trim().ToUpper());

            // Soyad girildiyse doğrula
            if (booking != null && !string.IsNullOrWhiteSpace(surname) &&
                !booking.Passengers.Any(p => p.Surname.Equals(surname.Trim(), StringComparison.OrdinalIgnoreCase)))
                booking = null;

            ViewBag.Flight = booking != null ? await _flightService.GetFlightByIdAsync(booking.FlightId) : null;
            return View(booking);
        }

        // Müşteri online check-in: PNR + soyad ile rezervasyonu getir, yolcuları göster
        [HttpGet]
        public async Task<IActionResult> CheckIn(string? pnr, string? surname)
        {
            ViewBag.Searched = !string.IsNullOrWhiteSpace(pnr);
            ViewBag.Pnr = pnr;
            ViewBag.Surname = surname;

            if (string.IsNullOrWhiteSpace(pnr))
                return View((Booking?)null);

            var booking = await _bookingService.GetByPnrAsync(pnr.Trim().ToUpper());
            if (booking != null && !string.IsNullOrWhiteSpace(surname) &&
                !booking.Passengers.Any(p => p.Surname.Equals(surname.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                booking = null;
                ViewBag.NotFound = true;
            }
            return View(booking);
        }

        // Müşteri check-in'i tamamla (koltuk + ek hizmet), sonra aynı PNR'ı tekrar göster
        [HttpPost]
        public async Task<IActionResult> CheckInComplete(string pnr, int passengerIndex, string seatNumber,
                                                         int extraBaggageKg, string? mealType, bool seatUpgrade)
        {
            await _checkInService.CheckInPassengerAsync(pnr, passengerIndex, seatNumber, extraBaggageKg, mealType, seatUpgrade);
            return RedirectToAction("CheckIn", new { pnr });
        }

        // Uçuş durumu sorgulama: uçuş numarasına göre uçuşları ve durumlarını göster
        [HttpGet]
        public async Task<IActionResult> FlightStatus(string? flightNumber)
        {
            ViewBag.Searched = !string.IsNullOrWhiteSpace(flightNumber);
            ViewBag.FlightNumber = flightNumber;

            var all = await _flightService.GetAllFlightsAsync();
            var results = string.IsNullOrWhiteSpace(flightNumber)
                ? all.OrderBy(x => x.DepartureTime).Take(10).ToList()
                : all.Where(x => x.FlightNumber.Contains(flightNumber.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            return View(results);
        }

        public IActionResult Confirmation()
        {
            ViewBag.Pnr = TempData["Pnr"] as string;
            return View();
        }
    }
}
