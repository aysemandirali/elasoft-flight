using FlightBooking.Entities;

namespace FlightBooking.Services.CheckInServices
{
    public interface ICheckInService
    {
        // PNR ile rezervasyonu (yolcularıyla birlikte) getir
        Task<Booking?> GetBookingByPnrAsync(string pnr);

        // Bir yolcuyu check-in yap: koltuk atar, ek hizmetleri (bagaj/yemek/koltuk)
        // uygular, biniş kartı üretir ve ayrı bir CheckIn kaydı (log) oluşturur.
        Task CheckInPassengerAsync(string pnr, int passengerIndex, string seatNumber,
                                   int extraBaggageKg, string? mealType, bool seatUpgrade);
    }
}
