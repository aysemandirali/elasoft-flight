using FlightBooking.Dtos.OverBookingDtos;
using FlightBooking.Services.MachineLearningServices;
using FlightBooking.Services.NoShowServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Areas.Admin.Controllers
{
    [Authorize]
    [Area("Admin")]
    public class OverbookingForecastController : Controller
    {
        private readonly OverbookingForecastService _forecastService;
        private readonly INoShowService _noShowService;

        public OverbookingForecastController(OverbookingForecastService forecastService, INoShowService noShowService)
        {
            _forecastService = forecastService;
            _noShowService = noShowService;
        }

        public async Task<IActionResult> Index(int month = 7, int capacity = 220, decimal averagePrice = 250)
        {
            var dataCount = (await _noShowService.GetAllAsync()).Count;
            ViewBag.DataCount = dataCount;
            ViewBag.Month = month;
            ViewBag.Capacity = capacity;
            ViewBag.AveragePrice = averagePrice;

            var results = dataCount >= 5
                ? _forecastService.Forecast(month, capacity, averagePrice)
                : new List<OverbookingForecastResultDto>();

            return View(results);
        }
    }
}
