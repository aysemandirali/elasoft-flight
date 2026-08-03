using FlightBooking.Dtos.BookingDtos;
using FlightBooking.Entities;

namespace FlightBooking.Services.BookingServices
{
    public interface IBookingService
    {
        Task<string> CreateBookingAsync(CreateBookingDto dto); // olusan PNR'yi dondurur
        Task<List<ResultBookingDto>> GetAllBookingsAsync();
        Task<Booking?> GetByPnrAsync(string pnr); // PNR ile rezervasyonu getir
    }
}
