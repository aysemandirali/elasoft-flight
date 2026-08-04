using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // MongoDB'deki "CarReservations" koleksiyonunda tutulan arac kiralama rezervasyonu.
    public class CarReservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string ReservationCode { get; set; } = string.Empty; // ARC-XXXXXX
        public string CarId { get; set; } = string.Empty;
        public string CarName { get; set; } = string.Empty;         // Renault Clio

        public string PickupLocation { get; set; } = string.Empty;
        public DateTime PickupDate { get; set; }
        public DateTime DropoffDate { get; set; }
        public int Days { get; set; }
        public decimal TotalPrice { get; set; }

        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }
}
