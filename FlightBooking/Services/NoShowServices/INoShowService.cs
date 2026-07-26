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

        // Ornek gecmis veriyi yukle. reset=true ise once mevcut veriyi siler.
        Task<int> SeedSampleDataAsync(bool reset = false);
    }
}
