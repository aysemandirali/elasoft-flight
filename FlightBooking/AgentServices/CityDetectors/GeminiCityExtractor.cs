namespace FlightBooking.AgentServices.CityDetectors
{
    public interface ICityExtractor
    {
        Task<string?> ExtractCityAsync(string prompt);
    }

    // Kullanicinin mesajindan sehir adini Gemini ile cikaran arac.
    public class GeminiCityExtractor : ICityExtractor
    {
        private readonly IGeminiService _gemini;

        public GeminiCityExtractor(IGeminiService gemini)
        {
            _gemini = gemini;
        }

        public async Task<string?> ExtractCityAsync(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return null;

            var city = await _gemini.GenerateAsync(
                "Kullanicinin mesajindan gitmek/seyahat etmek istedigi SEHRI bul. " +
                "SADECE sehir adini tek kelime olarak dondur. Sehir yoksa sadece 'NONE' yaz. Baska hicbir sey yazma.",
                prompt);

            city = city.Trim().Split('\n')[0].Trim().TrimEnd('.', ',');

            if (string.IsNullOrWhiteSpace(city) || city.Equals("NONE", StringComparison.OrdinalIgnoreCase) || city.Length > 40)
                return null;

            return city;
        }
    }
}
