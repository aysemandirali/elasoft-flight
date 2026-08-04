namespace FlightBooking.Dtos.DestinationDtos
{
    // Admin panelindeki Destinasyonlar sayfasinda gosterilen tek varis noktasi.
    // Ucus verilerinden uretilir.
    public class DestinationSummaryDto
    {
        public string Code { get; set; } = string.Empty;         // AYT
        public string Name { get; set; } = string.Empty;         // Antalya Havalimani
        public int FlightCount { get; set; }
        public int PassengerCount { get; set; }
        public List<string> Routes { get; set; } = new();        // buraya ucus yapan kalkis noktalari
    }
}
