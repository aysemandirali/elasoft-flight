using FlightBooking.Entities;

namespace FlightBooking.Services.TourServices
{
    public interface ITourService
    {
        Task SeedAsync();
        Task<List<Tour>> GetToursAsync(string? city, string? category);
        Task<List<string>> GetCitiesAsync();
        Task<Tour?> GetByIdAsync(string id);
        Task<TourReservation> CreateReservationAsync(TourReservation reservation);
        Task<TourReservation?> GetReservationByCodeAsync(string code);
        Task<List<TourReservation>> GetAllReservationsAsync();
    }
}
