namespace FlightBooking.Entities
{
    // Bir rezervasyon icindeki tek bir yolcu. Booking belgesinin icine gomulu tutulur.
    public class Passenger
    {
        public string PassengerId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;         // Erkek, Kadin
        public string PassengerType { get; set; } = string.Empty;  // Yetiskin, Cocuk, Bebek
        public string? SeatNumber { get; set; }
        public bool IsCheckedIn { get; set; }
        public DateTime? CheckInDate { get; set; }
        public string? TicketStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public string? CheckInStatus { get; set; }
        public int BaggageKg { get; set; }
        public string? MealType { get; set; }
        public List<string>? ExtraServices { get; set; }
        public string? BoardingPassNumber { get; set; }
        public string? Gate { get; set; }
        public DateTime? BoardingTime { get; set; }
    }
}
