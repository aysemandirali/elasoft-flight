using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // Tamamlanan bir check-in kaydi (log). "CheckIns" koleksiyonunda tutulur.
    public class CheckIn
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string CheckInId { get; set; } = string.Empty;

        public string PnrNumber { get; set; } = string.Empty;
        public string FlightId { get; set; } = string.Empty;

        // Yolcu adi (kolay okunsun diye kaydediyoruz)
        public string PassengerName { get; set; } = string.Empty;

        public DateTime CheckInDate { get; set; }
        public bool IsCheckedIn { get; set; }

        public string SeatNumber { get; set; } = string.Empty;
        public int BaggageKg { get; set; }
        public string? MealType { get; set; }

        // Satin alinan ek hizmetler
        public List<CheckInExtra> Extras { get; set; } = new();

        // Ek hizmetlerin toplam ucreti
        public decimal ExtraTotalPrice { get; set; }
    }
}
