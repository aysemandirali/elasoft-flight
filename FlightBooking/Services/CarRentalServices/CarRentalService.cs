using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Driver;

namespace FlightBooking.Services.CarRentalServices
{
    public class CarRentalService : ICarRentalService
    {
        private readonly IMongoCollection<Car> _cars;
        private readonly IMongoCollection<CarReservation> _reservations;

        public CarRentalService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _cars = db.GetCollection<Car>("Cars");
            _reservations = db.GetCollection<CarReservation>("CarReservations");
        }

        // Koleksiyon bossa ornek arac filosunu yukle.
        public async Task SeedAsync()
        {
            var any = await _cars.Find(x => true).AnyAsync();
            if (any) return;

            var cars = new List<Car>
            {
                new() { Brand = "Renault", Model = "Clio", Category = "Ekonomi", Transmission = "Manuel", Fuel = "Benzin", Seats = 5, LuggageCount = 2, DailyPrice = 850, Location = "İstanbul", Icon = "bi-car-front-fill" },
                new() { Brand = "Fiat", Model = "Egea", Category = "Ekonomi", Transmission = "Manuel", Fuel = "Dizel", Seats = 5, LuggageCount = 3, DailyPrice = 900, Location = "İstanbul", Icon = "bi-car-front-fill" },
                new() { Brand = "Volkswagen", Model = "Passat", Category = "Konfor", Transmission = "Otomatik", Fuel = "Dizel", Seats = 5, LuggageCount = 3, DailyPrice = 1650, Location = "İstanbul", Icon = "bi-car-front-fill" },
                new() { Brand = "Toyota", Model = "Corolla", Category = "Konfor", Transmission = "Otomatik", Fuel = "Hibrit", Seats = 5, LuggageCount = 3, DailyPrice = 1500, Location = "Ankara", Icon = "bi-car-front-fill" },
                new() { Brand = "Nissan", Model = "Qashqai", Category = "SUV", Transmission = "Otomatik", Fuel = "Dizel", Seats = 5, LuggageCount = 4, DailyPrice = 2100, Location = "Antalya", Icon = "bi-truck-front-fill" },
                new() { Brand = "Hyundai", Model = "Tucson", Category = "SUV", Transmission = "Otomatik", Fuel = "Benzin", Seats = 5, LuggageCount = 4, DailyPrice = 2250, Location = "İzmir", Icon = "bi-truck-front-fill" },
                new() { Brand = "Mercedes", Model = "E200", Category = "Lüks", Transmission = "Otomatik", Fuel = "Benzin", Seats = 5, LuggageCount = 3, DailyPrice = 3800, Location = "İstanbul", Icon = "bi-car-front-fill" },
                new() { Brand = "BMW", Model = "3.20i", Category = "Lüks", Transmission = "Otomatik", Fuel = "Benzin", Seats = 5, LuggageCount = 3, DailyPrice = 3600, Location = "Antalya", Icon = "bi-car-front-fill" },
                new() { Brand = "Tesla", Model = "Model 3", Category = "Lüks", Transmission = "Otomatik", Fuel = "Elektrik", Seats = 5, LuggageCount = 3, DailyPrice = 4200, Location = "İzmir", Icon = "bi-lightning-charge-fill" },
                new() { Brand = "Ford", Model = "Focus", Category = "Ekonomi", Transmission = "Manuel", Fuel = "Benzin", Seats = 5, LuggageCount = 2, DailyPrice = 950, Location = "Ankara", Icon = "bi-car-front-fill" },
            };

            await _cars.InsertManyAsync(cars);
        }

        public async Task<List<Car>> GetCarsAsync(string? location, string? category)
        {
            var builder = Builders<Car>.Filter;
            var filter = builder.Eq(x => x.IsAvailable, true);

            if (!string.IsNullOrWhiteSpace(location))
                filter &= builder.Eq(x => x.Location, location);
            if (!string.IsNullOrWhiteSpace(category) && category != "Tümü")
                filter &= builder.Eq(x => x.Category, category);

            return await _cars.Find(filter).SortBy(x => x.DailyPrice).ToListAsync();
        }

        public async Task<List<string>> GetLocationsAsync()
        {
            var cars = await _cars.Find(x => true).ToListAsync();
            return cars.Select(c => c.Location).Distinct().OrderBy(x => x).ToList();
        }

        public async Task<Car?> GetByIdAsync(string id)
        {
            return await _cars.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<CarReservation> CreateReservationAsync(CarReservation reservation)
        {
            reservation.ReservationCode = "ARC-" + GenerateCode();
            reservation.CreatedAt = DateTime.Now;
            await _reservations.InsertOneAsync(reservation);
            return reservation;
        }

        public async Task<CarReservation?> GetReservationByCodeAsync(string code)
        {
            return await _reservations.Find(x => x.ReservationCode == code).FirstOrDefaultAsync();
        }

        private static string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
