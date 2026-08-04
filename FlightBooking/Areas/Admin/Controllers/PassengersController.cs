using FlightBooking.Dtos.PassengerDtos;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.FlightServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class PassengersController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IFlightService _flightService;

        public PassengersController(IBookingService bookingService, IFlightService flightService)
        {
            _bookingService = bookingService;
            _flightService = flightService;
        }

        public async Task<IActionResult> Index()
        {
            var bookings = await _bookingService.GetAllRawAsync();
            var flights = await _flightService.GetAllFlightsAsync();

            // FlightId -> ucus bilgisi (rota ve numara icin)
            var flightMap = flights
                .GroupBy(f => f.FlightId)
                .ToDictionary(g => g.Key, g => g.First());

            var list = new List<AdminPassengerListDto>();

            foreach (var b in bookings)
            {
                flightMap.TryGetValue(b.FlightId, out var flight);

                foreach (var p in b.Passengers)
                {
                    list.Add(new AdminPassengerListDto
                    {
                        FullName = $"{p.Name} {p.Surname}".Trim(),
                        PassengerType = string.IsNullOrEmpty(p.PassengerType) ? "Yetişkin" : p.PassengerType,
                        Gender = p.Gender,
                        PnrNumber = b.PnrNumber,
                        ContactEmail = b.ContactEmail,
                        FlightNumber = flight?.FlightNumber ?? "-",
                        Route = flight != null ? $"{flight.DepartureAirportCode} → {flight.ArrivalAirportCode}" : "-",
                        SeatNumber = p.SeatNumber,
                        IsCheckedIn = p.IsCheckedIn,
                        BookingDate = b.BookingDate
                    });
                }
            }

            // En yeni rezervasyonlar üstte
            list = list.OrderByDescending(x => x.BookingDate).ToList();

            ViewBag.Total = list.Count;
            ViewBag.CheckedIn = list.Count(x => x.IsCheckedIn);

            return View(list);
        }
    }
}
