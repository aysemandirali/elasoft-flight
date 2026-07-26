namespace FlightBooking.Settings
{
    // appsettings.Local.json'daki "Gemini" bolumu buraya baglanir (anahtar gizli kalir).
    public class GeminiSettings
    {
        public string ApiKey { get; set; } = string.Empty;
        public string Model { get; set; } = "gemini-flash-latest";
    }
}
