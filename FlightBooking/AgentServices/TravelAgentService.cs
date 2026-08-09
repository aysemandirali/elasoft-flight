using FlightBooking.AgentServices.CityDetectors;
using FlightBooking.AgentServices.IntentDetectors;
using FlightBooking.AgentServices.PromptBuilders;

namespace FlightBooking.AgentServices
{
    // Agent'in urettigi sonuc (hangi adimlari yaptigini da tasir).
    public class AgentResult
    {
        public string Intent { get; set; } = string.Empty;
        public string? City { get; set; }
        public WeatherInfo? Weather { get; set; }
        public string Recommendation { get; set; } = string.Empty;
    }

    public interface ITravelAgentService
    {
        Task<AgentResult> AskAsync(string message);
    }

    // Moduler bilesenleri (niyet + sehir + arac + prompt + LLM) yoneten agent.
    public class TravelAgentService : ITravelAgentService
    {
        private readonly IIntentDetector _intentDetector;
        private readonly ICityExtractor _cityExtractor;
        private readonly IWeatherTool _weatherTool;
        private readonly ITravelPromptBuilder _promptBuilder;
        private readonly IGeminiService _gemini;

        public TravelAgentService(
            IIntentDetector intentDetector,
            ICityExtractor cityExtractor,
            IWeatherTool weatherTool,
            ITravelPromptBuilder promptBuilder,
            IGeminiService gemini)
        {
            _intentDetector = intentDetector;
            _cityExtractor = cityExtractor;
            _weatherTool = weatherTool;
            _promptBuilder = promptBuilder;
            _gemini = gemini;
        }

        public async Task<AgentResult> AskAsync(string message)
        {
            // 1) Niyet tespiti
            var intent = _intentDetector.Detect(message);

            // 2) Sehir cikarma
            var city = await _cityExtractor.ExtractCityAsync(message);

            // 3) Sehir varsa hava durumu aracini calistir
            WeatherInfo? weather = null;
            if (!string.IsNullOrWhiteSpace(city))
                weather = await _weatherTool.GetWeatherAsync(city);

            // 4) Prompt hazirla ve LLM'e sor
            string? weatherContext = weather != null
                ? $"{weather.City} icin hava: {weather.Temperature}°C, {weather.Description}."
                : null;

            var recommendation = await _gemini.GenerateAsync(
                _promptBuilder.BuildSystemPrompt(),
                _promptBuilder.BuildUserPrompt(message, weatherContext));

            // Yapay zeka servisine ulasilamazsa (kota/mesgul) elimizdeki
            // hava durumu verisinden yararli bir yedek cevap uret.
            if (recommendation.StartsWith("__ERROR__"))
                recommendation = BuildFallback(weather, city);

            return new AgentResult
            {
                Intent = intent.ToString(),
                City = weather?.City ?? city,
                Weather = weather,
                Recommendation = recommendation
            };
        }

        // LLM'e ulasilamadiginda hava durumu + sehir bilgisiyle basit oneri.
        private static string BuildFallback(WeatherInfo? weather, string? city)
        {
            if (weather != null)
            {
                var t = weather.Temperature;
                string tavsiye = t >= 25 ? "Hava oldukca sicak; yanina gunes gozlugu ve ince kiyafetler almani oneririm."
                    : t >= 15 ? "Hava iliman; mevsimlik bir ceket yeterli olacaktir."
                    : t >= 5 ? "Hava serin; yanina kalin bir mont almayi unutma."
                    : "Hava soguk; kalin giyinmeni ve yaninda atki-bere bulundurmani oneririm.";

                return $"{weather.City} icin guncel hava durumu {t}°C ve {weather.Description}. {tavsiye} " +
                       $"Diledigin tarihte {weather.City} ucuslarini 'Ucus Ara' bolumunden inceleyebilirsin. " +
                       "(Not: Yapay zeka asistani su an yogun oldugundan bu oneri otomatik olusturuldu.)";
            }

            var yer = string.IsNullOrWhiteSpace(city) ? "gitmek istedigin sehir" : city;
            return $"{yer} hakkinda sana yardimci olmak isterim. Su an yapay zeka asistani yogun; " +
                   "birkac dakika sonra tekrar deneyebilir ya da 'Ucus Ara' bolumunden uygun ucuslari inceleyebilirsin.";
        }
    }
}
