using FlightBooking.Entities;
using FlightBooking.Settings;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;

namespace FlightBooking.Services.AccountServices
{
    public class AuthService : IAuthService
    {
        private readonly IMongoCollection<AppUser> _users;
        private readonly PasswordHasher<AppUser> _hasher = new();

        public AuthService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<AppUser>("Users");
        }

        // Yeni kullanici olustur (email zaten varsa false doner). Sifre hashlenerek saklanir.
        public async Task<bool> RegisterAsync(string fullName, string email, string password)
        {
            email = email.Trim().ToLower();
            var exists = await _users.Find(x => x.Email == email).AnyAsync();
            if (exists) return false;

            var user = new AppUser { FullName = fullName.Trim(), Email = email };
            user.PasswordHash = _hasher.HashPassword(user, password);
            await _users.InsertOneAsync(user);
            return true;
        }

        // Sistemde hic admin yoksa varsayilan bir admin hesabi olustur.
        public async Task EnsureDefaultAdminAsync()
        {
            const string adminEmail = "admin@elasoft.com";
            var exists = await _users.Find(x => x.Email == adminEmail).AnyAsync();
            if (exists) return;

            var admin = new AppUser { FullName = "Sistem Yöneticisi", Email = adminEmail, Role = "Admin" };
            admin.PasswordHash = _hasher.HashPassword(admin, "Admin123!");
            await _users.InsertOneAsync(admin);
        }

        // Email + sifre dogruysa kullaniciyi dondur, degilse null.
        public async Task<AppUser?> ValidateLoginAsync(string email, string password)
        {
            email = email.Trim().ToLower();
            var user = await _users.Find(x => x.Email == email).FirstOrDefaultAsync();
            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return result == PasswordVerificationResult.Failed ? null : user;
        }
    }
}
