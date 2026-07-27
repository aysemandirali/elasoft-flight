namespace FlightBooking.AgentServices
{
    // Agent'in urettigi sonuc (hangi adimlari yaptigini da tasir).
    public class AgentResult
    {
        public string? City { get; set; }
        public WeatherInfo? Weather { get; set; }
        public string Recommendation { get; set; } = string.Empty;
    }

    public interface ITravelAgentService
    {
        Task<AgentResult> AskAsync(string message);
    }

    // LLM (Gemini) + arac (hava durumu) + karar zincirini yoneten agent.
    public class TravelAgentService : ITravelAgentService
    {
        private readonly IGeminiService _gemini;
        private readonly IWeatherTool _weatherTool;

        public TravelAgentService(IGeminiService gemini, IWeatherTool weatherTool)
        {
            _gemini = gemini;
            _weatherTool = weatherTool;
        }

        public async Task<AgentResult> AskAsync(string message)
        {
            // 1) ADIM: Mesajdan gidilmek istenen sehri cikar (city extraction)
            var city = await _gemini.GenerateAsync(
                "Sen bir varlik cikarma aracisin. Kullanicinin mesajindan gitmek/seyahat etmek istedigi SEHRI bul. " +
                "SADECE sehir adini tek kelime olarak dondur. Sehir yoksa sadece 'YOK' yaz. Baska hicbir sey yazma.",
                message);
            city = city.Trim().Split('\n')[0].Trim().TrimEnd('.', ',');

            // 2) ADIM: Sehir bulunduysa hava durumu aracini calistir (tool calling)
            WeatherInfo? weather = null;
            if (!string.IsNullOrWhiteSpace(city) && !city.Equals("YOK", StringComparison.OrdinalIgnoreCase) && city.Length <= 40)
            {
                weather = await _weatherTool.GetWeatherAsync(city);
            }

            // 3) ADIM: Toplanan bilgilerle LLM'e nihai oneriyi olustur
            var context = weather != null
                ? $"\n\n[Arac verisi] {weather.City} icin hava: {weather.Temperature}°C, {weather.Description}. Bu hava durumuna uygun oneriler de ekle."
                : "";

            var recommendation = await _gemini.GenerateAsync(
                "Sen bir ucus rezervasyon sitesinin Turkce seyahat asistanisin. Kullaniciya kisa, samimi ve pratik " +
                "seyahat onerileri ver (gezilecek yerler, ne yapilir, ne giyilir). Sana bir arac verisi (hava durumu) " +
                "verildiyse onu mutlaka dikkate al. Cevabin Turkce olsun.",
                message + context);

            return new AgentResult
            {
                City = weather?.City ?? (city.Equals("YOK", StringComparison.OrdinalIgnoreCase) ? null : city),
                Weather = weather,
                Recommendation = recommendation
            };
        }
    }
}
