using FlightBooking.Dtos.PassengerDtos;

namespace FlightBooking.Dtos.BookingDtos
{
    // Yeni rezervasyon formundan gelen veri.
    public class CreateBookingDto
    {
        public string FlightId { get; set; } = string.Empty;
        public List<CreatePassengerDto> Passengers { get; set; } = new();
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
    }
}
