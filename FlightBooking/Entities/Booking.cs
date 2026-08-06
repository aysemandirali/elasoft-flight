using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // MongoDB'deki "Bookings" koleksiyonundaki bir rezervasyon.
    // Yolcular ayri tablo degil; bu belgenin icine liste olarak gomulur.
    public class Booking
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string BookingId { get; set; } = string.Empty;

        public string FlightId { get; set; } = string.Empty;      // hangi ucus
        public string PnrNumber { get; set; } = string.Empty;     // 6 haneli rezervasyon kodu
        public List<Passenger> Passengers { get; set; } = new();

        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;

        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;        // Confirmed, Cancelled
        public string PaymentStatus { get; set; } = "Bekliyor";   // Bekliyor, Ödendi
    }
}
