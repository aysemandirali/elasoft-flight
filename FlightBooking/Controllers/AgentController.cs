using FlightBooking.AgentServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    // Halka acik AI seyahat asistani (arac kullanan agent).
    public class AgentController : Controller
    {
        private readonly ITravelAgentService _agent;

        public AgentController(ITravelAgentService agent)
        {
            _agent = agent;
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
                var result = await _agent.AskAsync(message);
                ViewBag.Intent = result.Intent;
                ViewBag.City = result.City;
                ViewBag.Weather = result.Weather; // WeatherInfo?
                ViewBag.Answer = result.Recommendation;
            }
            return View();
        }
    }
}
