using Microsoft.ML.Data;

namespace FlightBooking.MachineLearningModels
{
    // Talep tahmini modeline giren ozellikler (etiket: SoldTickets).
    public class FlightDemandInput
    {
        public string FlightSlot { get; set; } = string.Empty;
        public float Month { get; set; }
        public float Capacity { get; set; }
        public float SoldTickets { get; set; } // tahmin edilecek deger (talep)
    }

    public class FlightDemandPrediction
    {
        [ColumnName("Score")]
        public float PredictedDemand { get; set; }
    }
}
