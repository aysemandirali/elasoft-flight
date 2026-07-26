using FlightBooking.Dtos.OverBookingDtos;
using FlightBooking.Entities;

namespace FlightBooking.Services.NoShowServices
{
    public interface INoShowService
    {
        Task<List<NoShowHistory>> GetAllAsync();

        // Slot bazli ortalama no-show oranlari (yüzde): { "Morning-1": 5.2, ... }
        Task<Dictionary<string, double>> GetSlotBasedNoShowRatesAsync();

        // Bir slot icin overbooking onerisi hesapla
        Task<OverbookingRecommendationResult> GenerateRecommendationAsync(string flightSlot, int forecastPassenger, int capacity);

        // Koleksiyon bossa ornek gecmis veriyi yukle
        Task<int> SeedSampleDataAsync();
    }
}
