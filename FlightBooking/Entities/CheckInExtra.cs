namespace FlightBooking.Entities
{
    // Check-in sirasinda satin alinan tek bir ek hizmet.
    public class CheckInExtra
    {
        public string ExtraType { get; set; } = string.Empty; // Baggage, Meal, Seat
        public string ExtraName { get; set; } = string.Empty; // "Ek Bagaj 10kg" gibi
        public decimal Price { get; set; }
    }
}
