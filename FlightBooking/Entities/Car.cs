using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // MongoDB'deki "Cars" koleksiyonunda tutulan kiralik arac.
    public class Car
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string Brand { get; set; } = string.Empty;        // Renault
        public string Model { get; set; } = string.Empty;        // Clio
        public string Category { get; set; } = string.Empty;     // Ekonomi, Konfor, SUV, Lüks
        public string Transmission { get; set; } = string.Empty; // Manuel, Otomatik
        public string Fuel { get; set; } = string.Empty;         // Benzin, Dizel, Elektrik
        public int Seats { get; set; }
        public int LuggageCount { get; set; }
        public decimal DailyPrice { get; set; }
        public string Location { get; set; } = string.Empty;     // teslim alma sehri
        public string Icon { get; set; } = "bi-car-front-fill";  // arayuzde gosterilecek ikon
        public bool IsAvailable { get; set; } = true;
    }
}
