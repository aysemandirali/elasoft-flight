namespace FlightBooking.Dtos.FlightDtos
{
    // Ucus listesinde gosterilen veri.
    public class ResultFlightDto
    {
        public string FlightId { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string AirlineCode { get; set; } = string.Empty;
        public string DepartureAirportCode { get; set; } = string.Empty;
        public string DepartureAirportName { get; set; } = string.Empty;
        public string ArrivalAirportCode { get; set; } = string.Empty;
        public string ArrivalAirportName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public int DurationMinutes { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public decimal BasePrice { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
