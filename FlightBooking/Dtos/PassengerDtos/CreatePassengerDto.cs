namespace FlightBooking.Dtos.PassengerDtos
{
    // Rezervasyon formunda girilen tek bir yolcu.
    public class CreatePassengerDto
    {
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string PassengerType { get; set; } = string.Empty;
    }
}
