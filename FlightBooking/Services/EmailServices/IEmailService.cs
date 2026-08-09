using FlightBooking.Dtos.FlightDtos;
using FlightBooking.Entities;

namespace FlightBooking.Services.EmailServices
{
    public interface IEmailService
    {
        // Rezervasyon onay (PNR) e-postasi gonderir. Hata durumunda akisi bozmaz.
        Task SendBookingConfirmationAsync(Booking booking, GetFlightByIdDto? flight);
    }
}
