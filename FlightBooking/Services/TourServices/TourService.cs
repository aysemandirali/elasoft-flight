using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Driver;

namespace FlightBooking.Services.TourServices
{
    public class TourService : ITourService
    {
        private readonly IMongoCollection<Tour> _tours;
        private readonly IMongoCollection<TourReservation> _reservations;

        public TourService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(settings.DatabaseName);
            _tours = db.GetCollection<Tour>("Tours");
            _reservations = db.GetCollection<TourReservation>("TourReservations");
        }

        public async Task SeedAsync()
        {
            var any = await _tours.Find(x => true).AnyAsync();
            if (any) return;

            var tours = new List<Tour>
            {
                new() { Title = "Kapadokya Balon Turu", City = "Nevşehir", Category = "Aktivite", Duration = "3 saat", PricePerPerson = 4500, Icon = "bi-balloon", Description = "Gün doğumunda peri bacaları üzerinde unutulmaz bir balon deneyimi." },
                new() { Title = "İstanbul Boğaz Turu", City = "İstanbul", Category = "Tur", Duration = "2 saat", PricePerPerson = 750, Icon = "bi-water", Description = "Tekneyle Boğaz'ın iki yakasını ve tarihi yalıları keşfedin." },
                new() { Title = "Efes Antik Kenti Gezisi", City = "İzmir", Category = "Gezi", Duration = "Yarım gün", PricePerPerson = 1200, Icon = "bi-bank", Description = "Rehber eşliğinde antik Efes'in tarihine yolculuk." },
                new() { Title = "Pamukkale & Hierapolis", City = "Denizli", Category = "Gezi", Duration = "1 gün", PricePerPerson = 1800, Icon = "bi-droplet", Description = "Beyaz travertenler ve antik havuzda gün boyu keşif." },
                new() { Title = "Antalya Tekne & Dalış", City = "Antalya", Category = "Aktivite", Duration = "1 gün", PricePerPerson = 1600, Icon = "bi-life-preserver", Description = "Akdeniz'in turkuaz koylarında yüzme ve dalış turu." },
                new() { Title = "Fethiye Yamaç Paraşütü", City = "Muğla", Category = "Aktivite", Duration = "1 saat", PricePerPerson = 3200, Icon = "bi-airplane-engines", Description = "Ölüdeniz üzerinde profesyonel pilot eşliğinde uçuş." },
                new() { Title = "Sultanahmet Yürüyüş Turu", City = "İstanbul", Category = "Tur", Duration = "3 saat", PricePerPerson = 500, Icon = "bi-signpost-split", Description = "Ayasofya, Sultanahmet ve Topkapı çevresinde rehberli tur." },
                new() { Title = "Bodrum Gece Turu", City = "Muğla", Category = "Tur", Duration = "4 saat", PricePerPerson = 900, Icon = "bi-moon-stars", Description = "Bodrum'un ünlü marinasında akşam eğlence turu." },
            };

            await _tours.InsertManyAsync(tours);
        }

        public async Task<List<Tour>> GetToursAsync(string? city, string? category)
        {
            var builder = Builders<Tour>.Filter;
            var filter = builder.Eq(x => x.IsAvailable, true);

            if (!string.IsNullOrWhiteSpace(city))
                filter &= builder.Eq(x => x.City, city);
            if (!string.IsNullOrWhiteSpace(category) && category != "Tümü")
                filter &= builder.Eq(x => x.Category, category);

            return await _tours.Find(filter).SortBy(x => x.PricePerPerson).ToListAsync();
        }

        public async Task<List<string>> GetCitiesAsync()
        {
            var tours = await _tours.Find(x => true).ToListAsync();
            return tours.Select(t => t.City).Distinct().OrderBy(x => x).ToList();
        }

        public async Task<Tour?> GetByIdAsync(string id)
        {
            // Bozuk bir kimlik gelirse sorgu calistirilmaz.
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _)) return null;
            return await _tours.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<TourReservation> CreateReservationAsync(TourReservation reservation)
        {
            reservation.ReservationCode = "TUR-" + GenerateCode();
            reservation.CreatedAt = DateTime.Now;
            await _reservations.InsertOneAsync(reservation);
            return reservation;
        }

        public async Task<TourReservation?> GetReservationByCodeAsync(string code)
        {
            return await _reservations.Find(x => x.ReservationCode == code).FirstOrDefaultAsync();
        }

        public async Task<List<TourReservation>> GetAllReservationsAsync()
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
