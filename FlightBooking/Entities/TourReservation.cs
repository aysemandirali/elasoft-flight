using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // MongoDB'deki "TourReservations" koleksiyonunda tutulan tur rezervasyonu.
    public class TourReservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string ReservationCode { get; set; } = string.Empty; // TUR-XXXXXX
        public string TourId { get; set; } = string.Empty;
        public string TourTitle { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;

        public DateTime Date { get; set; }
        public int PersonCount { get; set; }
        public decimal TotalPrice { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
