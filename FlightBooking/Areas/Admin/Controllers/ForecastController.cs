using FlightBooking.Services.MachineLearningServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class ForecastController : Controller
    {
        private readonly NoShowMlService _mlService;

        public ForecastController(NoShowMlService mlService)
        {
            _mlService = mlService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // Modeli egit (gecmis veriyle)
        [HttpPost]
        public IActionResult Train()
        {
            var ok = _mlService.Train();
            TempData["TrainMessage"] = ok
                ? $"Model {_mlService.TrainedRecordCount} kayıtla eğitildi ✔"
                : "Eğitim için yeterli veri yok (önce Overbooking sayfasından örnek veri yükleyin).";
            return RedirectToAction("Index");
        }

        // Tahmin yap
        [HttpPost]
        public IActionResult Predict(string flightSlot, float soldTickets, float capacity)
        {
            var predicted = _mlService.Predict(flightSlot, soldTickets, capacity);
            ViewBag.Predicted = Math.Round(predicted, 1);
            ViewBag.FlightSlot = flightSlot;
            ViewBag.SoldTickets = soldTickets;
            ViewBag.Capacity = capacity;
            ViewBag.TrainedCount = _mlService.TrainedRecordCount;
            return View("Index");
        }
    }
}
