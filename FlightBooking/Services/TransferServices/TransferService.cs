using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Driver;

namespace FlightBooking.Services.TransferServices
{
    public class TransferService : ITransferService
    {
        private readonly IMongoCollection<TransferVehicle> _vehicles;
        private readonly IMongoCollection<TransferReservation> _reservations;

        public TransferService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _vehicles = db.GetCollection<TransferVehicle>("TransferVehicles");
            _reservations = db.GetCollection<TransferReservation>("TransferReservations");
        }

        public async Task SeedAsync()
        {
            var any = await _vehicles.Find(x => true).AnyAsync();
            if (any) return;

            var vehicles = new List<TransferVehicle>
            {
                new() { VehicleType = "Ekonomi Sedan", Capacity = 3, Price = 650, Icon = "bi-car-front-fill", Description = "3 yolcu + bagaj için ekonomik özel transfer." },
                new() { VehicleType = "Konfor Sedan", Capacity = 4, Price = 850, Icon = "bi-car-front-fill", Description = "Geniş iç hacimli konforlu binek araç." },
                new() { VehicleType = "VIP Vito", Capacity = 6, Price = 1450, Icon = "bi-truck-front-fill", Description = "Deri koltuklu, geniş VIP minivan." },
                new() { VehicleType = "Minibüs", Capacity = 12, Price = 2200, Icon = "bi-bus-front-fill", Description = "Kalabalık gruplar için 12 kişilik minibüs." },
            };

            await _vehicles.InsertManyAsync(vehicles);
        }

        public async Task<List<TransferVehicle>> GetVehiclesAsync()
        {
            return await _vehicles.Find(x => x.IsAvailable).SortBy(x => x.Price).ToListAsync();
        }

        public async Task<TransferVehicle?> GetByIdAsync(string id)
        {
            // Bozuk bir kimlik gelirse sorgu calistirilmaz.
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return null;
            return await _vehicles.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<TransferReservation> CreateReservationAsync(TransferReservation reservation)
        {
            reservation.ReservationCode = "TRF-" + GenerateCode();
            reservation.CreatedAt = DateTime.Now;
            await _reservations.InsertOneAsync(reservation);
            return reservation;
        }

        public async Task<TransferReservation?> GetReservationByCodeAsync(string code)
        {
            return await _reservations.Find(x => x.ReservationCode == code).FirstOrDefaultAsync();
        }

        public async Task<List<TransferReservation>> GetAllReservationsAsync()
        {
            return await _reservations.Find(x => true).SortByDescending(x => x.CreatedAt).ToListAsync();
        }

        private static string GenerateCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
