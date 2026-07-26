using FlightBooking.Entities;

namespace FlightBooking.Services.CheckInServices
{
    public interface ICheckInService
    {
        // PNR ile rezervasyonu (yolcularıyla birlikte) getir
        Task<Booking?> GetBookingByPnrAsync(string pnr);

        // Rezervasyondaki belirli bir yolcuyu check-in yap (koltuk atar, biniş kartı üretir)
        Task CheckInPassengerAsync(string pnr, int passengerIndex, string seatNumber);
    }
}
