namespace FlightBooking.AgentServices.PromptBuilders
{
    public interface ITravelPromptBuilder
    {
        string BuildSystemPrompt();
        string BuildUserPrompt(string userMessage, string? weatherContext);
    }

    // Asistanin sistem talimatini ve kullanici promptunu hazirlayan sinif.
    public class TravelPromptBuilder : ITravelPromptBuilder
    {
        public string BuildSystemPrompt() =>
            "Sen bir ucus rezervasyon sitesinin profesyonel Turkce seyahat asistanisin. " +
            "Kullaniciya kisa, samimi ve pratik oneriler ver (gezilecek yerler, ne yapilir, ne giyilir, ne yenir). " +
            "Sana bir arac verisi (hava durumu) verildiyse onu mutlaka dikkate al. Cevabin Turkce olsun.";

        public string BuildUserPrompt(string userMessage, string? weatherContext)
        {
            if (string.IsNullOrEmpty(weatherContext))
                return userMessage;

            return $"{userMessage}\n\n[Arac verisi] {weatherContext} Bu hava durumuna uygun oneriler de ekle.";
        }
    }
}
