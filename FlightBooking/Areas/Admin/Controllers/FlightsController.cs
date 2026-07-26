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
    }
}
