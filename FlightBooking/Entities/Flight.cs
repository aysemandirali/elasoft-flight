using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace FlightBooking.Entities
{
    // MongoDB'deki "Flights" koleksiyonunda tutulan bir ucusu temsil eder.
    public class Flight
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string FlightId { get; set; } = string.Empty; // MongoDB ObjectId

        public string FlightNumber { get; set; } = string.Empty;          // TK123, PC2023
        public string AirlineCode { get; set; } = string.Empty;           // TK, PC, LH
        public string DepartureAirportCode { get; set; } = string.Empty;  // IST
        public string DepartureAirportName { get; set; } = string.Empty;  // Istanbul Havalimani
        public string ArrivalAirportCode { get; set; } = string.Empty;    // LHR
        public string ArrivalAirportName { get; set; } = string.Empty;    // London Heathrow
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public decimal BasePrice { get; set; }
        public string Currency { get; set; } = string.Empty;             // TRY, EUR, USD
        public string Status { get; set; } = string.Empty;               // Scheduled, Delayed, Cancelled, Completed
    }
}
