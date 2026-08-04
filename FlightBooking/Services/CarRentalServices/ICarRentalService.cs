using FlightBooking.Entities;

namespace FlightBooking.Services.CarRentalServices
{
    public interface ICarRentalService
    {
        Task SeedAsync();                                            // ilk acilista ornek araclari yukle
        Task<List<Car>> GetCarsAsync(string? location, string? category);
        Task<List<string>> GetLocationsAsync();                     // teslim noktalari (arama kutusu icin)
        Task<Car?> GetByIdAsync(string id);
        Task<CarReservation> CreateReservationAsync(CarReservation reservation);
        Task<CarReservation?> GetReservationByCodeAsync(string code);
    }
}
