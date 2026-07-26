using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // Gecmis ucuslarin no-show (ucusa gelmeyen yolcu) kayitlari.
    // Overbooking onerisi bu gecmisten hesaplanir.
    public class NoShowHistory
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; } = string.Empty;

        [BsonElement("route")]
        public string Route { get; set; } = string.Empty;          // SAW-BGY

        [BsonElement("flightDate")]
        public string FlightDate { get; set; } = string.Empty;      // 2026-01-01

        [BsonElement("flightSlot")]
        public string FlightSlot { get; set; } = string.Empty;      // Morning-1, Evening-1...

        [BsonElement("aircraftType")]
        public string AircraftType { get; set; } = string.Empty;

        [BsonElement("capacity")]
        public int Capacity { get; set; }

        [BsonElement("soldTickets")]
        public int SoldTickets { get; set; }

        [BsonElement("onlineCheckedIn")]
        public int OnlineCheckedIn { get; set; }

        [BsonElement("airportCheckedIn")]
        public int AirportCheckedIn { get; set; }

        [BsonElement("boardedPassenger")]
        public int BoardedPassenger { get; set; }

        [BsonElement("noShowPassenger")]
        public int NoShowPassenger { get; set; }

        [BsonElement("onlineCheckInNoShow")]
        public int OnlineCheckInNoShow { get; set; }

        [BsonElement("missedConnection")]
        public int MissedConnection { get; set; }

        [BsonElement("cancelledPassenger")]
        public int CancelledPassenger { get; set; }
    }
}
