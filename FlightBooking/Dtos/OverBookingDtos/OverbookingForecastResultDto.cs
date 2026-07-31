namespace FlightBooking.Dtos.OverBookingDtos
{
    // Birleşik overbooking tahmin panelinde bir slot'un satırı.
    public class OverbookingForecastResultDto
    {
        public string FlightSlot { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int ForecastPassenger { get; set; }     // talep modeli: kaç bilet satılır
        public int PredictedNoShow { get; set; }       // no-show modeli: kaç kişi gelmez
        public int RecommendedMaxSale { get; set; }    // kapasite + beklenen no-show
        public int ExtraSeatCount { get; set; }        // fazladan satılabilir koltuk
        public string RiskLevel { get; set; } = "Low"; // Low / Medium / High
        public decimal EstimatedRevenue { get; set; }  // tahmini gelir
    }
}
