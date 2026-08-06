using FlightBooking.Entities;
using FlightBooking.Services.TourServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    // Turlar & Aktiviteler (müşteri tarafı, giriş gerektirmez).
    public class TourController : Controller
    {
        private readonly ITourService _tourService;

        public TourController(ITourService tourService)
        {
            _tourService = tourService;
        }

        public async Task<IActionResult> Index(string? city, string? category)
        {
            var tours = await _tourService.GetToursAsync(city, category);
            ViewBag.Cities = await _tourService.GetCitiesAsync();
            ViewBag.City = city;
            ViewBag.Category = category ?? "Tümü";
            return View(tours);
        }

        public async Task<IActionResult> Reserve(string id)
        {
            var tour = await _tourService.GetByIdAsync(id);
            if (tour == null) return RedirectToAction("Index");
            return View(tour);
        }

        [HttpPost]
        public async Task<IActionResult> ReserveComplete(string tourId, DateTime date, int personCount,
            string customerName, string customerEmail, string customerPhone)
        {
            var tour = await _tourService.GetByIdAsync(tourId);
            if (tour == null) return RedirectToAction("Index");

            if (personCount < 1) personCount = 1;

            var reservation = new TourReservation
            {
                TourId = tour.Id,
                TourTitle = tour.Title,
                City = tour.City,
                Date = date,
                PersonCount = personCount,
                TotalPrice = personCount * tour.PricePerPerson,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone
            };

            var saved = await _tourService.CreateReservationAsync(reservation);
            return RedirectToAction("Confirmation", new { code = saved.ReservationCode });
        }

        public async Task<IActionResult> Confirmation(string code)
        {
            var reservation = await _tourService.GetReservationByCodeAsync(code);
            if (reservation == null) return RedirectToAction("Index");
            return View(reservation);
        }
    }
}
