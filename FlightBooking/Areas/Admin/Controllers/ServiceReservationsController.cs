using FlightBooking.Services.CarRentalServices;
using FlightBooking.Services.TourServices;
using FlightBooking.Services.TransferServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    // Araç kiralama, tur ve transfer rezervasyonlarını tek panelde listeler.
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ServiceReservationsController : Controller
    {
        private readonly ICarRentalService _carService;
        private readonly ITourService _tourService;
        private readonly ITransferService _transferService;

        public ServiceReservationsController(ICarRentalService carService, ITourService tourService, ITransferService transferService)
        {
            _carService = carService;
            _tourService = tourService;
            _transferService = transferService;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _carService.GetAllReservationsAsync();
            var tours = await _tourService.GetAllReservationsAsync();
            var transfers = await _transferService.GetAllReservationsAsync();

            ViewBag.CarReservations = cars;
            ViewBag.TourReservations = tours;
            ViewBag.TransferReservations = transfers;

            ViewBag.CarCount = cars.Count;
            ViewBag.TourCount = tours.Count;
            ViewBag.TransferCount = transfers.Count;

            var revenue = cars.Sum(x => x.TotalPrice) + tours.Sum(x => x.TotalPrice) + transfers.Sum(x => x.TotalPrice);
            ViewBag.Revenue = revenue.ToString("#,##0", new System.Globalization.CultureInfo("tr-TR"));

            return View();
        }
    }
}
