using FlightBooking.Dtos.BookingDtos;
using FlightBooking.Entities;

namespace FlightBooking.Services.BookingServices
{
    public interface IBookingService
    {
        Task<string> CreateBookingAsync(CreateBookingDto dto); // olusan PNR'yi dondurur
        Task<List<ResultBookingDto>> GetAllBookingsAsync();
        Task<List<Booking>> GetAllRawAsync(); // gomulu yolcularla birlikte ham rezervasyonlar
        Task<Booking?> GetByPnrAsync(string pnr); // PNR ile rezervasyonu getir
        Task MarkAsPaidAsync(string pnr);         // odemeyi tamamla (PaymentStatus = Ödendi)
    }
}
