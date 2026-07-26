using FlightBooking.AgentServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    // Halka acik AI seyahat asistani sohbet ekrani.
    public class AgentController : Controller
    {
        private readonly IGeminiService _geminiService;

        public AgentController(IGeminiService geminiService)
        {
            _geminiService = geminiService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                ViewBag.Question = message;
                ViewBag.Answer = await _geminiService.AskAsync(message);
            }
            return View();
        }
    }
}
