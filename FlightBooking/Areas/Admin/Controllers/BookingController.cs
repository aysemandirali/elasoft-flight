using FlightBooking.Dtos.BookingDtos;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.FlightServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IFlightService _flightService;

        public BookingController(IBookingService bookingService, IFlightService flightService)
        {
            _bookingService = bookingService;
            _flightService = flightService;
        }

        // Tum rezervasyonlari listele
        public async Task<IActionResult> BookingList()
        {
            var values = await _bookingService.GetAllBookingsAsync();
            return View(values);
        }

        // Yeni rezervasyon formu — ucuslari acilir listeye doldur
        [HttpGet]
        public async Task<IActionResult> CreateBooking()
        {
            await FillFlightsDropdown();
            return View();
        }

        // Formu kaydet, sonra listeye don
        [HttpPost]
        public async Task<IActionResult> CreateBooking(CreateBookingDto createBookingDto)
        {
            await _bookingService.CreateBookingAsync(createBookingDto);
            return RedirectToAction("BookingList");
        }

        // Acilir listeyi ucuslarla doldurur (ViewBag ile view'a gecer)
        private async Task FillFlightsDropdown()
        {
            var flights = await _flightService.GetAllFlightsAsync();
            ViewBag.Flights = flights.Select(x => new SelectListItem
            {
                Value = x.FlightId,
                Text = $"{x.FlightNumber} — {x.DepartureAirportCode} > {x.ArrivalAirportCode} ({x.BasePrice} {x.Currency})"
            }).ToList();
        }
    }
}
