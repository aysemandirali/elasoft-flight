using FlightBooking.Dtos.BookingDtos;

namespace FlightBooking.Services.BookingServices
{
    public interface IBookingService
    {
        Task<string> CreateBookingAsync(CreateBookingDto dto); // olusan PNR'yi dondurur
        Task<List<ResultBookingDto>> GetAllBookingsAsync();
    }
}
