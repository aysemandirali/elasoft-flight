using FlightBooking.Entities;
using FlightBooking.Services.TransferServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    // Havalimanı Transferi (müşteri tarafı, giriş gerektirmez).
    public class TransferController : Controller
    {
        private readonly ITransferService _transferService;

        public TransferController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        // Araç seçenekleri + transfer arama formu (tek sayfa)
        public async Task<IActionResult> Index()
        {
            var vehicles = await _transferService.GetVehiclesAsync();
            return View(vehicles);
        }

        // Seçilen araç için rezervasyon formu (transfer detaylarıyla)
        public async Task<IActionResult> Reserve(string id, string? from, string? to)
        {
            var vehicle = await _transferService.GetByIdAsync(id);
            if (vehicle == null) return RedirectToAction("Index");
            ViewBag.From = from;
            ViewBag.To = to;
            return View(vehicle);
        }

        [HttpPost]
        public async Task<IActionResult> ReserveComplete(string vehicleId, string fromLocation, string toLocation,
            DateTime date, string time, int passengerCount, bool roundTrip,
            string customerName, string customerEmail, string customerPhone)
        {
            var vehicle = await _transferService.GetByIdAsync(vehicleId);
            if (vehicle == null) return RedirectToAction("Index");

            if (passengerCount < 1) passengerCount = 1;
            var total = roundTrip ? vehicle.Price * 2 : vehicle.Price;

            var reservation = new TransferReservation
            {
                VehicleId = vehicle.Id,
                VehicleType = vehicle.VehicleType,
                FromLocation = fromLocation,
                ToLocation = toLocation,
                Date = date,
                Time = time,
                PassengerCount = passengerCount,
                RoundTrip = roundTrip,
                TotalPrice = total,
                CustomerName = customerName,
                CustomerEmail = customerEmail,
                CustomerPhone = customerPhone
            };

            var saved = await _transferService.CreateReservationAsync(reservation);
            return RedirectToAction("Confirmation", new { code = saved.ReservationCode });
        }

        public async Task<IActionResult> Confirmation(string code)
        {
            var reservation = await _transferService.GetReservationByCodeAsync(code);
            if (reservation == null) return RedirectToAction("Index");
            return View(reservation);
        }
    }
}
