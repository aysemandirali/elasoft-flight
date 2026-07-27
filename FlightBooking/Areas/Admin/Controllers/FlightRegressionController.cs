using FlightBooking.Services.MachineLearningServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class FlightRegressionController : Controller
    {
        private readonly FlightDemandMlService _demandService;

        public FlightRegressionController(FlightDemandMlService demandService)
        {
            _demandService = demandService;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpPost]
        public IActionResult Train()
        {
            var ok = _demandService.Train();
            TempData["TrainMessage"] = ok
                ? $"Model {_demandService.TrainedRecordCount} kayıtla eğitildi ✔"
                : "Eğitim için yeterli veri yok (önce Overbooking sayfasından örnek veri yükleyin).";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Predict(string flightSlot, int month, float capacity)
        {
            var predicted = _demandService.Predict(flightSlot, month, capacity);
            ViewBag.Predicted = (int)Math.Round(predicted);
            ViewBag.FlightSlot = flightSlot;
            ViewBag.Month = month;
            ViewBag.Capacity = capacity;
            ViewBag.TrainedCount = _demandService.TrainedRecordCount;
            ViewBag.LoadFactor = capacity > 0 ? Math.Round(predicted / capacity * 100, 1) : 0;
            return View("Index");
        }
    }
}
