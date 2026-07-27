using System.Text.Json;

namespace FlightBooking.AgentServices
{
    // Bir sehrin anlik hava durumu (agent'in kullandigi arac / tool).
    public class WeatherInfo
    {
        public string City { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public interface IWeatherTool
    {
        Task<WeatherInfo?> GetWeatherAsync(string city);
    }

    public class WeatherTool : IWeatherTool
    {
        private readonly HttpClient _httpClient;

        public WeatherTool(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<WeatherInfo?> GetWeatherAsync(string city)
        {
            try
            {
                // 1) Sehir adindan koordinat bul (ucretsiz, anahtarsiz)
                var geoUrl = $"https://geocoding-api.open-meteo.com/v1/search?name={Uri.EscapeDataString(city)}&count=1&language=tr";
                using var geoDoc = JsonDocument.Parse(await _httpClient.GetStringAsync(geoUrl));
                if (!geoDoc.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
                    return null;

                var first = results[0];
                var lat = first.GetProperty("latitude").GetDouble();
                var lon = first.GetProperty("longitude").GetDouble();
                var name = first.GetProperty("name").GetString() ?? city;

                // 2) Koordinattan anlik hava durumu al
                var wUrl = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current=temperature_2m,weather_code";
                using var wDoc = JsonDocument.Parse(await _httpClient.GetStringAsync(wUrl));
                var current = wDoc.RootElement.GetProperty("current");
                var temp = current.GetProperty("temperature_2m").GetDouble();
                var code = current.GetProperty("weather_code").GetInt32();

                return new WeatherInfo
                {
                    City = name,
                    Temperature = temp,
                    Description = DescribeCode(code)
                };
            }
            catch
            {
                return null; // hava durumu alinamazsa agent onsuz devam eder
            }
        }

        // WMO hava kodunu Turkce aciklamaya cevir
        private static string DescribeCode(int code) => code switch
        {
            0 => "açık",
            1 or 2 or 3 => "parçalı bulutlu",
            45 or 48 => "sisli",
            51 or 53 or 55 or 56 or 57 => "çiseli",
            61 or 63 or 65 or 66 or 67 => "yağmurlu",
            71 or 73 or 75 or 77 => "karlı",
            80 or 81 or 82 => "sağanak yağışlı",
            95 or 96 or 99 => "gök gürültülü fırtına",
            _ => "değişken"
        };
    }
}
