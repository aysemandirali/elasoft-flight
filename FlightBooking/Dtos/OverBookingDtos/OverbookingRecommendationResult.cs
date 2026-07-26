namespace FlightBooking.Dtos.OverBookingDtos
{
    // Overbooking oneri sonucu (bir slot icin).
    public class OverbookingRecommendationResult
    {
        public string FlightSlot { get; set; } = string.Empty;
        public int ForecastPassengerCount { get; set; }
        public int Capacity { get; set; }
        public double ExpectedNoShowRate { get; set; }      // yüzde
        public int ExpectedNoShowPassenger { get; set; }
        public int RecommendedMaxTicketSale { get; set; }
        public int ExtraSellableSeatCount { get; set; }
        public string RiskLevel { get; set; } = "Low";      // Low / Medium / High
        public string Recommendation { get; set; } = string.Empty;
    }
}
