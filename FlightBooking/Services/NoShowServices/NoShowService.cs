using FlightBooking.Dtos.OverBookingDtos;
using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Driver;

namespace FlightBooking.Services.NoShowServices
{
    public class NoShowService : INoShowService
    {
        private readonly IMongoCollection<NoShowHistory> _collection;

        public NoShowService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<NoShowHistory>(settings.NoShowHistoryCollection);
        }

        public async Task<List<NoShowHistory>> GetAllAsync()
        {
            return await _collection.Find(x => true).ToListAsync();
        }

        public async Task<Dictionary<string, double>> GetSlotBasedNoShowRatesAsync()
        {
            var all = await GetAllAsync();

            // Slot'a gore grupla, her grupta ortalama (noShow / soldTickets * 100)
            return all
                .Where(x => x.SoldTickets > 0)
                .GroupBy(x => x.FlightSlot)
                .ToDictionary(
                    g => g.Key,
                    g => Math.Round(g.Average(x => (double)x.NoShowPassenger / x.SoldTickets * 100), 2)
                );
        }

        public async Task<OverbookingRecommendationResult> GenerateRecommendationAsync(string flightSlot, int forecastPassenger, int capacity)
        {
            var slotRates = await GetSlotBasedNoShowRatesAsync();
            double noShowRate = slotRates.TryGetValue(flightSlot, out var r) ? r : 0;

            int expectedNoShow = (int)Math.Round(forecastPassenger * (noShowRate / 100));
            int recommendedMaxSale = capacity + expectedNoShow;

            string riskLevel = noShowRate >= 7 ? "High" : noShowRate >= 5 ? "Medium" : "Low";
            string recommendation = riskLevel switch
            {
                "High" => "Agresif overbooking uygulanabilir",
                "Medium" => "Kontrollü overbooking önerilir",
                _ => "Standart satış politikası önerilir"
            };

            return new OverbookingRecommendationResult
            {
                FlightSlot = flightSlot,
                ForecastPassengerCount = forecastPassenger,
                Capacity = capacity,
                ExpectedNoShowRate = noShowRate,
                ExpectedNoShowPassenger = expectedNoShow,
                RecommendedMaxTicketSale = recommendedMaxSale,
                ExtraSellableSeatCount = recommendedMaxSale - capacity,
                RiskLevel = riskLevel,
                Recommendation = recommendation
            };
        }

        public async Task<int> SeedSampleDataAsync(bool reset = false)
        {
            if (reset)
                await _collection.DeleteManyAsync(x => true); // mevcut veriyi temizle

            var count = await _collection.CountDocumentsAsync(x => true);
            if (count > 0) return 0; // zaten veri var

            var slots = new[] { "Morning-1", "Morning-2", "Afternoon-1", "Evening-1", "Night-1" };
            var rnd = new Random(42);
            var records = new List<NoShowHistory>();

            // Her slot icin 6 gunluk kayit uret; slotlarin no-show egilimi farkli
            var baseNoShow = new Dictionary<string, int>
            {
                ["Morning-1"] = 6, ["Morning-2"] = 9, ["Afternoon-1"] = 12,
                ["Evening-1"] = 16, ["Night-1"] = 20
            };

            // Slot bazli temel talep (dolu/bos egilimi)
            var baseDemand = new Dictionary<string, int>
            {
                ["Morning-1"] = 212, ["Morning-2"] = 206, ["Afternoon-1"] = 196,
                ["Evening-1"] = 216, ["Night-1"] = 184
            };

            // 12 ay boyunca, her ay birkac gun, her slot icin kayit uret
            for (int month = 1; month <= 12; month++)
            {
                // Mevsim etkisi: yaz aylari talep yuksek, kis dusuk
                int monthFactor = (month is 6 or 7 or 8) ? 10 : (month is 12 or 1 or 2) ? -6 : 0;

                foreach (var day in new[] { 3, 10, 18, 25 })
                {
                    foreach (var slot in slots)
                    {
                        int capacity = 220;
                        int sold = baseDemand[slot] + monthFactor + rnd.Next(-4, 5);
                        sold = Math.Clamp(sold, 120, capacity);
                        int noShow = baseNoShow[slot] + rnd.Next(-2, 3);
                        if (noShow < 0) noShow = 0;
                        int boarded = sold - noShow;
                        int online = (int)(boarded * 0.6);
                        int airport = boarded - online;

                        records.Add(new NoShowHistory
                        {
                            Route = "SAW-BGY",
                            FlightDate = $"2026-{month:D2}-{day:D2}",
                            FlightSlot = slot,
                            AircraftType = "Airbus A321",
                            Capacity = capacity,
                            SoldTickets = sold,
                            OnlineCheckedIn = online,
                            AirportCheckedIn = airport,
                            BoardedPassenger = boarded,
                            NoShowPassenger = noShow,
                            OnlineCheckInNoShow = rnd.Next(0, 4),
                            MissedConnection = rnd.Next(0, 3),
                            CancelledPassenger = rnd.Next(0, 2)
                        });
                    }
                }
            }

            await _collection.InsertManyAsync(records);
            return records.Count;
        }
    }
}
