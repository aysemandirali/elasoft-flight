using FlightBooking.Services.CheckInServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class CheckInController : Controller
    {
        private readonly ICheckInService _checkInService;

        public CheckInController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        // PNR arama formu (bos)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // PNR ile rezervasyonu bul ve yolcularini goster
        [HttpPost]
        public async Task<IActionResult> Index(string pnr)
        {
            var booking = await _checkInService.GetBookingByPnrAsync(pnr?.Trim().ToUpper() ?? "");
            if (booking == null)
                ViewBag.Message = "Bu PNR ile rezervasyon bulunamadı.";

            ViewBag.Pnr = pnr;
            return View(booking);
        }

        // Bir yolcuyu check-in yap, sonra ayni PNR'yi tekrar goster
        [HttpPost]
        public async Task<IActionResult> Complete(string pnr, int passengerIndex, string seatNumber)
        {
            await _checkInService.CheckInPassengerAsync(pnr, passengerIndex, seatNumber);
            var booking = await _checkInService.GetBookingByPnrAsync(pnr);
            ViewBag.Pnr = pnr;
            ViewBag.Message = "Check-in tamamlandı ✔";
            return View("Index", booking);
        }
    }
}
