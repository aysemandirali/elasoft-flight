namespace FlightBooking.Dtos.BookingDtos
{
    // Admin rezervasyon listesinde gosterilen ozet veri.
    public class ResultBookingDto
    {
        public string BookingId { get; set; } = string.Empty;
        public string PnrNumber { get; set; } = string.Empty;
        public string FlightId { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public int PassengerCount { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
