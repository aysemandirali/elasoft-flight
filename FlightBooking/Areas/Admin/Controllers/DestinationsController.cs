using FlightBooking.Dtos.DestinationDtos;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.FlightServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class DestinationsController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;

        public DestinationsController(IFlightService flightService, IBookingService bookingService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            var flights = await _flightService.GetAllFlightsAsync();
            var bookings = await _bookingService.GetAllRawAsync();

            // Her varış noktası için o noktaya giden uçuşların yolcu sayısını topla
            var passengerByFlight = bookings
                .GroupBy(b => b.FlightId)
                .ToDictionary(g => g.Key, g => g.Sum(b => b.Passengers.Count));

            var destinations = flights
                .GroupBy(f => f.ArrivalAirportCode)
                .Select(g => new DestinationSummaryDto
                {
                    Code = g.Key,
                    Name = g.First().ArrivalAirportName,
                    FlightCount = g.Count(),
                    PassengerCount = g.Sum(f => passengerByFlight.TryGetValue(f.FlightId, out var c) ? c : 0),
                    Routes = g.Select(f => f.DepartureAirportCode).Distinct().OrderBy(x => x).ToList()
                })
                .OrderByDescending(x => x.FlightCount)
                .ToList();

            return View(destinations);
        }
    }
}
