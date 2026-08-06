using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // MongoDB'deki "TransferVehicles" koleksiyonunda tutulan havalimanı transfer aracı seçeneği.
    public class TransferVehicle
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string VehicleType { get; set; } = string.Empty; // Ekonomi Sedan, VIP Vito, Minibüs
        public int Capacity { get; set; }                        // yolcu kapasitesi
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }                       // tek yön baz ücret
        public string Icon { get; set; } = "bi-car-front";
        public bool IsAvailable { get; set; } = true;
    }
}
