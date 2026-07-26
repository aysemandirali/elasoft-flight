using System.Text;
using System.Text.Json;
using FlightBooking.Settings;
using Microsoft.Extensions.Options;

namespace FlightBooking.AgentServices
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly GeminiSettings _settings;

        // Asistanin kisiligi/gorevi
        private const string SystemPrompt =
            "Sen bir ucus rezervasyon sitesinin Turkce seyahat asistanisin. " +
            "Kullaniciya ucuslar, destinasyonlar, seyahat onerileri ve rezervasyon konularinda " +
            "kisa, samimi ve yardimci cevaplar ver. Cevaplarin Turkce olsun.";

        public GeminiService(HttpClient httpClient, IOptions<GeminiSettings> options)
        {
            _httpClient = httpClient;
            _settings = options.Value;
        }

        public async Task<string> AskAsync(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
                return "API anahtari ayarlanmamis. appsettings.Local.json dosyasini kontrol et.";

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{_settings.Model}:generateContent";

            var payload = new
            {
                system_instruction = new { parts = new[] { new { text = SystemPrompt } } },
                contents = new[]
                {
                    new { parts = new[] { new { text = userMessage } } }
                }
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Add("X-goog-api-key", _settings.ApiKey);
            request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return "Asistana su an ulasilamadi. (Hata kodu: " + (int)response.StatusCode + ")";

            // Cevaptan metni ayikla: candidates[0].content.parts[0].text
            try
            {
                using var doc = JsonDocument.Parse(body);
                var text = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();
                return text ?? "(bos cevap)";
            }
            catch
            {
                return "Cevap islenirken bir sorun olustu.";
            }
        }
    }
}
