using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // MongoDB'deki "Users" koleksiyonunda tutulan panel kullanicisi.
    public class AppUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty; // sifre asla duz metin tutulmaz
        public string Role { get; set; } = "Customer"; // Admin veya Customer
    }
}
