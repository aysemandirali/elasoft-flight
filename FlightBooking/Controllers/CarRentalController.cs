using FlightBooking.Entities;
using FlightBooking.Services.CarRentalServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    // Araç kiralama (müşteri tarafı, giriş gerektirmez).
    public class CarRentalController : Controller
    {
        private readonly ICarRentalService _carService;

        public CarRentalController(ICarRentalService carService)
        {
            _carService = carService;
        }

        // Arama + araç listesi
        public async Task<IActionResult> Index(string? location, string? category, DateTime? pickupDate, DateTime? dropoffDate)
        {
            var cars = await _carService.GetCarsAsync(location, category);

            ViewBag.Locations = await _carService.GetLocationsAsync();
            ViewBag.Location = location;
            ViewBag.Category = category ?? "Tümü";
            ViewBag.PickupDate = (pickupDate ?? DateTime.Today).ToString("yyyy-MM-dd");
            ViewBag.DropoffDate = (dropoffDate ?? DateTime.Today.AddDays(3)).ToString("yyyy-MM-dd");

            return View(cars);
        }

        // Rezervasyon formu
        public async Task<IActionResult> Reserve(string id, DateTime? pickupDate, DateTime? dropoffDate)
        {
            var car = await _carService.GetByIdAsync(id);
            if (car == null) return RedirectToAction("Index");

            var pick = pickupDate ?? DateTime.Today;
            var drop = dropoffDate ?? DateTime.Today.AddDays(3);
            var days = Math.Max(1, (drop.Date - pick.Date).Days);

            ViewBag.PickupDate = pick.ToString("yyyy-MM-dd");
            ViewBag.DropoffDate = drop.ToString("yyyy-MM-dd");
            ViewBag.Days = days;
            ViewBag.Total = days * car.DailyPrice;

            return View(car);
        }

        [HttpPost]
        public async Task<IActionResult> ReserveComplete(string carId, DateTime pickupDate, DateTime dropoffDate,
            string customerName, string customerEmail, string customerPhone)
        {
            var car = await _carService.GetByIdAsync(carId);
            if (car == null) return RedirectToAction("Index");

            var days = Math.Max(1, (dropoffDate.Date - pickupDate.Date).Days);

            var reservation = new CarReservation
            {
                CarId = car.Id,
                CarName = $"{car.Brand} {car.Model}",
                PickupLocation = car.Location,
                PickupDate = pickupDate,
                DropoffDate = dropoffDate,
                Days = days,
                TotalPrice = days * car.DailyPrice,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone
            };

            var saved = await _carService.CreateReservationAsync(reservation);
            return RedirectToAction("Confirmation", new { code = saved.ReservationCode });
        }

        public async Task<IActionResult> Confirmation(string code)
        {
            var reservation = await _carService.GetReservationByCodeAsync(code);
            if (reservation == null) return RedirectToAction("Index");
            return View(reservation);
        }
    }
}
