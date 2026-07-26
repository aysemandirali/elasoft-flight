using FlightBooking.Dtos.FlightDtos;
using FlightBooking.Services.FlightServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FlightsController : Controller
    {
        private readonly IFlightService _flightService;

        public FlightsController(IFlightService flightService)
        {
            _flightService = flightService;
        }

        // Ucus listesi sayfasi
        public async Task<IActionResult> FlightList()
        {
            var values = await _flightService.GetAllFlightsAsync();
            return View(values);
        }

        // Yeni ucus formunu goster
        [HttpGet]
        public IActionResult CreateFlight()
        {
            return View();
        }

        // Formu kaydet, sonra listeye don
        [HttpPost]
        public async Task<IActionResult> CreateFlight(CreateFlightDto createFlightDto)
        {
            await _flightService.CreateFlightAsync(createFlightDto);
            return RedirectToAction("FlightList");
        }

        // Ucus guncelleme formunu, secili ucusun bilgileriyle doldurup goster
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var flight = await _flightService.GetFlightByIdAsync(id);
            var model = new UpdateFlightDto
            {
                FlightId = flight.FlightId,
                FlightNumber = flight.FlightNumber,
                AirlineCode = flight.AirlineCode,
                DepartureAirportCode = flight.DepartureAirportCode,
                DepartureAirportName = flight.DepartureAirportName,
                ArrivalAirportCode = flight.ArrivalAirportCode,
                ArrivalAirportName = flight.ArrivalAirportName,
                DepartureTime = flight.DepartureTime,
                ArrivalTime = flight.ArrivalTime,
                DurationMinutes = flight.DurationMinutes,
                TotalSeats = flight.TotalSeats,
                AvailableSeats = flight.AvailableSeats,
                BasePrice = flight.BasePrice,
                Currency = flight.Currency,
                Status = flight.Status
            };
            return View(model);
        }

        // Guncellemeyi kaydet
        [HttpPost]
        public async Task<IActionResult> Edit(UpdateFlightDto updateFlightDto)
        {
            await _flightService.UpdateFlightAsync(updateFlightDto);
            return RedirectToAction("FlightList");
        }

        // Ucusu sil
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await _flightService.DeleteFlightAsync(id);
            return RedirectToAction("FlightList");
        }

        // Ucus detayini goster
        public async Task<IActionResult> FlightDetail(string id)
        {
            var flight = await _flightService.GetFlightByIdAsync(id);
            return View(flight);
        }
    }
}
