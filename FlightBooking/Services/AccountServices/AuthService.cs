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

        // Sistemde yoksa varsayilan admin ve demo musteri hesaplarini olustur.
        public async Task EnsureDefaultAdminAsync()
        {
            await EnsureUserAsync("admin@elasoft.com", "Sistem Yöneticisi", "Admin123!", "Admin");
            await EnsureUserAsync("demo@elasoft.com", "Demo Müşteri", "Demo123!", "Customer");
        }

        // Belirtilen e-posta yoksa verilen bilgilerle kullanici olustur.
        private async Task EnsureUserAsync(string email, string fullName, string password, string role)
        {
            var exists = await _users.Find(x => x.Email == email).AnyAsync();
            if (exists) return;

            var user = new AppUser { FullName = fullName, Email = email, Role = role };
            user.PasswordHash = _hasher.HashPassword(user, password);
            await _users.InsertOneAsync(user);
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

        // Email ile kullaniciyi getir (profil sayfasi icin).
        public async Task<AppUser?> GetByEmailAsync(string email)
        {
            email = email.Trim().ToLower();
            return await _users.Find(x => x.Email == email).FirstOrDefaultAsync();
        }

        // Mevcut sifre dogruysa yeni sifreyi hashleyip kaydet.
        public async Task<bool> ChangePasswordAsync(string email, string currentPassword, string newPassword)
        {
            email = email.Trim().ToLower();
            var user = await _users.Find(x => x.Email == email).FirstOrDefaultAsync();
            if (user == null) return false;

            var check = _hasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (check == PasswordVerificationResult.Failed) return false;

            var newHash = _hasher.HashPassword(user, newPassword);
            var update = Builders<AppUser>.Update.Set(x => x.PasswordHash, newHash);
            await _users.UpdateOneAsync(x => x.Id == user.Id, update);
            return true;
        }
    }
}
