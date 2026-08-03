using FlightBooking.Entities;

namespace FlightBooking.Services.AccountServices
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string fullName, string email, string password);
        Task<AppUser?> ValidateLoginAsync(string email, string password);
        Task EnsureDefaultAdminAsync();
    }
}
