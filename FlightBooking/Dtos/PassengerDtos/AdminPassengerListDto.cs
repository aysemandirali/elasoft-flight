namespace FlightBooking.Dtos.PassengerDtos
{
    // Admin panelindeki Yolcular sayfasinda gosterilen tek satir.
    // Rezervasyon icine gomulu yolculardan uretilir.
    public class AdminPassengerListDto
    {
        public string FullName { get; set; } = string.Empty;
        public string PassengerType { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string PnrNumber { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string Route { get; set; } = string.Empty;          // IST → AYT
        public string? SeatNumber { get; set; }
        public bool IsCheckedIn { get; set; }
        public DateTime BookingDate { get; set; }
    }
}
