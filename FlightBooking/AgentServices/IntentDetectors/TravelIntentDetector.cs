namespace FlightBooking.AgentServices.IntentDetectors
{
    // Kullanicinin mesajindaki seyahat niyeti.
    public enum TravelIntent
    {
        Unknown,
        Destination,   // bir yere gitmek/seyahat
        Weather,       // hava durumu
        Restaurant,    // yemek/restoran
        Hotel,         // konaklama
        Attraction     // gezilecek yer
    }

    public interface IIntentDetector
    {
        TravelIntent Detect(string prompt);
    }

    // Anahtar kelimelere gore basit niyet tespiti.
    public class TravelIntentDetector : IIntentDetector
    {
        public TravelIntent Detect(string prompt)
        {
            if (string.IsNullOrWhiteSpace(prompt))
                return TravelIntent.Unknown;

            var p = prompt.ToLower();

            if (p.Contains("restoran") || p.Contains("yemek") || p.Contains("nerede yenir") || p.Contains("kahvaltı"))
                return TravelIntent.Restaurant;

            if (p.Contains("hava") || p.Contains("sıcaklık") || p.Contains("yağmur") || p.Contains("derece"))
                return TravelIntent.Weather;

            if (p.Contains("otel") || p.Contains("konaklama") || p.Contains("kalacak"))
                return TravelIntent.Hotel;

            if (p.Contains("gezilecek") || p.Contains("müze") || p.Contains("tarihi") || p.Contains("görülecek"))
                return TravelIntent.Attraction;

            if (p.Contains("git") || p.Contains("seyahat") || p.Contains("tatil") || p.Contains("uç") || p.Contains("gez"))
                return TravelIntent.Destination;

            return TravelIntent.Unknown;
        }
    }
}
