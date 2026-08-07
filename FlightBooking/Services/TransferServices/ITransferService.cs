using FlightBooking.Entities;

namespace FlightBooking.Services.TransferServices
{
    public interface ITransferService
    {
        Task SeedAsync();
        Task<List<TransferVehicle>> GetVehiclesAsync();
        Task<TransferVehicle?> GetByIdAsync(string id);
        Task<TransferReservation> CreateReservationAsync(TransferReservation reservation);
        Task<TransferReservation?> GetReservationByCodeAsync(string code);
        Task<List<TransferReservation>> GetAllReservationsAsync();
    }
}
