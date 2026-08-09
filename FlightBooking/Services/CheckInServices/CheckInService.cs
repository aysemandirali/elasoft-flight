using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Driver;

namespace FlightBooking.Services.CheckInServices
{
    public class CheckInService : ICheckInService
    {
        private readonly IMongoCollection<Booking> _bookingCollection;
        private readonly IMongoCollection<CheckIn> _checkInCollection;

        public CheckInService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _bookingCollection = database.GetCollection<Booking>(settings.BookingCollectionName);
            _checkInCollection = database.GetCollection<CheckIn>(settings.CheckInCollectionName);
        }

        public async Task<Booking?> GetBookingByPnrAsync(string pnr)
        {
            return await _bookingCollection.Find(x => x.PnrNumber == pnr).FirstOrDefaultAsync();
        }

        public async Task CheckInPassengerAsync(string pnr, int passengerIndex, string seatNumber,
                                                int extraBaggageKg, string? mealType, bool seatUpgrade)
        {
            var booking = await GetBookingByPnrAsync(pnr);
            if (booking == null) return;
            if (passengerIndex < 0 || passengerIndex >= booking.Passengers.Count) return;

            var passenger = booking.Passengers[passengerIndex];

            // 1) Ek hizmetleri ve ucretlerini sunucuda hesapla
            var extras = new List<CheckInExtra>();
            if (extraBaggageKg > 0)
                extras.Add(new CheckInExtra { ExtraType = "Baggage", ExtraName = $"Ek Bagaj {extraBaggageKg} kg", Price = extraBaggageKg * 15m });
            if (!string.IsNullOrEmpty(mealType) && mealType != "Yok")
                extras.Add(new CheckInExtra { ExtraType = "Meal", ExtraName = $"Yemek: {mealType}", Price = MealPrice(mealType) });
            if (seatUpgrade)
                extras.Add(new CheckInExtra { ExtraType = "Seat", ExtraName = "Ön Sıra Koltuk Yükseltme", Price = 100m });

            var extraTotal = extras.Sum(x => x.Price);

            // 2) Yolcuyu guncelle
            passenger.IsCheckedIn = true;
            passenger.CheckInStatus = "Checked-In";
            passenger.CheckInDate = DateTime.Now;
            passenger.SeatNumber = seatNumber;
            passenger.TicketStatus = "Issued";
            passenger.BaggageKg = extraBaggageKg;
            passenger.MealType = mealType;
            passenger.ExtraServices = extras.Select(x => x.ExtraName).ToList();
            passenger.BoardingPassNumber = "BP-" + pnr + "-" + (passengerIndex + 1);
            passenger.Gate = new[] { "A1", "A2", "B5", "C3", "D7" }[new Random().Next(5)];
            passenger.BoardingTime = DateTime.Now.AddMinutes(45);

            await _bookingCollection.ReplaceOneAsync(x => x.BookingId == booking.BookingId, booking);

            // 3) CheckIn kaydini (log) olustur
            var checkIn = new CheckIn
            {
                PnrNumber = pnr,
                FlightId = booking.FlightId,
                PassengerName = $"{passenger.Name} {passenger.Surname}",
                CheckInDate = DateTime.Now,
                IsCheckedIn = true,
                SeatNumber = seatNumber,
                BaggageKg = extraBaggageKg,
                MealType = mealType,
                Extras = extras,
                ExtraTotalPrice = extraTotal
            };
            await _checkInCollection.InsertOneAsync(checkIn);
        }

        // Ek hizmet toplam ucreti (odeme adiminda kullanilir)
        public decimal CalculateExtraCost(int extraBaggageKg, string? mealType, bool seatUpgrade)
        {
            decimal total = 0m;
            if (extraBaggageKg > 0) total += extraBaggageKg * 15m;
            if (!string.IsNullOrEmpty(mealType) && mealType != "Yok") total += MealPrice(mealType);
            if (seatUpgrade) total += 100m;
            return total;
        }

        // Yemek tipine gore ucret
        private static decimal MealPrice(string mealType) => mealType switch
        {
            "Standart" => 50m,
            "Vejetaryen" => 80m,
            "Premium Menü" => 150m,
            _ => 0m
        };
    }
}
