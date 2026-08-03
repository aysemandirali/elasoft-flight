using FlightBooking.Services.NoShowServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class OverBookingController : Controller
    {
        private readonly INoShowService _noShowService;

        public OverBookingController(INoShowService noShowService)
        {
            _noShowService = noShowService;
        }

        // Slot bazli no-show oranlarini ve oneri formunu goster
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ViewBag.Rates = await _noShowService.GetSlotBasedNoShowRatesAsync();
            ViewBag.RecordCount = (await _noShowService.GetAllAsync()).Count;
            return View();
        }

        // Overbooking onerisi hesapla
        [HttpPost]
        public async Task<IActionResult> Recommend(string flightSlot, int forecastPassenger, int capacity)
        {
            ViewBag.Rates = await _noShowService.GetSlotBasedNoShowRatesAsync();
            ViewBag.RecordCount = (await _noShowService.GetAllAsync()).Count;
            ViewBag.Result = await _noShowService.GenerateRecommendationAsync(flightSlot, forecastPassenger, capacity);
            return View("Index");
        }

        // Ornek gecmis veriyi yukle
        [HttpPost]
        public async Task<IActionResult> Seed(bool reset = false)
        {
            var added = await _noShowService.SeedSampleDataAsync(reset);
            TempData["SeedMessage"] = added > 0 ? $"{added} örnek kayıt yüklendi." : "Zaten veri mevcut.";
            return RedirectToAction("Index");
        }
    }
}
