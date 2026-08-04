using FlightBooking.Entities;

namespace FlightBooking.Services.AccountServices
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(string fullName, string email, string password);
        Task<AppUser?> ValidateLoginAsync(string email, string password);
        Task EnsureDefaultAdminAsync();
        Task<AppUser?> GetByEmailAsync(string email);
        Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword);
    }
}
