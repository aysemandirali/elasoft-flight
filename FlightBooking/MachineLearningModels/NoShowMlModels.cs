using Microsoft.ML.Data;

namespace FlightBooking.MachineLearningModels
{
    // Modele giren ozellikler (ve egitimde etiket olarak NoShowPassenger).
    public class NoShowMlInput
    {
        public string FlightSlot { get; set; } = string.Empty;
        public float SoldTickets { get; set; }
        public float Capacity { get; set; }
        public float NoShowPassenger { get; set; } // etiket (tahmin edilecek deger)
    }

    // Modelin ciktisi.
    public class NoShowMlPrediction
    {
        [ColumnName("Score")]
        public float PredictedNoShow { get; set; }
    }
}
