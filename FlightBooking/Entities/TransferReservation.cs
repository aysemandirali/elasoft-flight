using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // MongoDB'deki "TransferReservations" koleksiyonunda tutulan havalimanı transfer rezervasyonu.
    public class TransferReservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string ReservationCode { get; set; } = string.Empty; // TRF-XXXXXX
        public string VehicleId { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;

        public string FromLocation { get; set; } = string.Empty;   // örn: İstanbul Havalimanı (IST)
        public string ToLocation { get; set; } = string.Empty;     // örn: Taksim
        public DateTime Date { get; set; }
        public string Time { get; set; } = string.Empty;
        public int PassengerCount { get; set; }
        public bool RoundTrip { get; set; }
        public decimal TotalPrice { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
