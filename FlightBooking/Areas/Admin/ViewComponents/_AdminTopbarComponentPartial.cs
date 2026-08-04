using FlightBooking.Services.BookingServices;
using FlightBooking.Services.FlightServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.ViewComponents
{
    // Topbar; bildirimleri gerçek uçuş/rezervasyon verisinden üretir.
    public class _AdminTopbarComponentPartial : ViewComponent
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;

        public _AdminTopbarComponentPartial(IFlightService flightService, IBookingService bookingService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var flights = await _flightService.GetAllFlightsAsync();
            var bookings = await _bookingService.GetAllBookingsAsync();

            var notifications = new List<(string Icon, string Color, string Text)>();

            foreach (var f in flights.Where(x => x.Status == "Delayed").Take(3))
                notifications.Add(("bi-exclamation-triangle-fill", "#F59E0B", $"{f.FlightNumber} gecikmeli — {f.DepartureAirportCode}→{f.ArrivalAirportCode}"));

            foreach (var f in flights.Where(x => x.Status == "Cancelled").Take(2))
                notifications.Add(("bi-x-circle-fill", "#EF4444", $"{f.FlightNumber} iptal edildi"));

            // Son 24 saatteki yeni rezervasyonlar
            var recent = bookings.Count(b => b.BookingDate >= System.DateTime.Now.AddDays(-1));
            if (recent > 0)
                notifications.Add(("bi-person-check-fill", "#10B981", $"{recent} yeni rezervasyon (son 24 saat)"));

            // Bugün kalkışı olan uçuşlar
            var todayFlights = flights.Count(f => f.DepartureTime.Date == System.DateTime.Today);
            if (todayFlights > 0)
                notifications.Add(("bi-airplane-fill", "#1E6FD9", $"Bugün {todayFlights} uçuş planlı"));

            ViewBag.Notifications = notifications;
            ViewBag.NotifCount = notifications.Count;

            return View();
        }
    }
}
