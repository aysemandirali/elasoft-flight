using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // MongoDB'deki "Tours" koleksiyonunda tutulan tur/aktivite.
    public class Tour
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;       // Kapadokya Balon Turu
        public string City { get; set; } = string.Empty;        // Nevşehir
        public string Category { get; set; } = string.Empty;    // Tur, Aktivite, Gezi
        public string Duration { get; set; } = string.Empty;    // "3 saat", "1 gün"
        public decimal PricePerPerson { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = "bi-compass";
        public bool IsAvailable { get; set; } = true;
    }
}
