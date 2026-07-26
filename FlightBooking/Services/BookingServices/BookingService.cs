using FlightBooking.Dtos.BookingDtos;
using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Driver;

namespace FlightBooking.Services.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly IMongoCollection<Booking> _bookingCollection;
        private readonly IMongoCollection<Flight> _flightCollection;

        public BookingService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _bookingCollection = database.GetCollection<Booking>(settings.BookingCollectionName);
            _flightCollection = database.GetCollection<Flight>(settings.FlightCollectionName);
        }

        public async Task<string> CreateBookingAsync(CreateBookingDto dto)
        {
            // Once ucusu bul (fiyat hesabi icin)
            var flight = await _flightCollection.Find(x => x.FlightId == dto.FlightId).FirstOrDefaultAsync();

            // Form yolcularini entity'ye cevir
            var passengers = dto.Passengers.Select(x => new Passenger
            {
                Name = x.Name,
                Surname = x.Surname,
                BirthDate = x.BirthDate,
                Gender = x.Gender,
                PassengerType = x.PassengerType
            }).ToList();

            // Toplam fiyat = yolcu sayisi * temel fiyat
            var basePrice = flight?.BasePrice ?? 0;
            var totalPrice = passengers.Count * basePrice;

            var booking = new Booking
            {
                FlightId = dto.FlightId,
                Passengers = passengers,
                ContactName = dto.ContactName,
                ContactEmail = dto.ContactEmail,
                ContactPhone = dto.ContactPhone,
                TotalPrice = totalPrice,
                BookingDate = DateTime.Now,
                Status = "Confirmed",
                PnrNumber = await GenerateUniquePnrAsync()
            };

            await _bookingCollection.InsertOneAsync(booking);
            return booking.PnrNumber;
        }

        public async Task<List<ResultBookingDto>> GetAllBookingsAsync()
        {
            var bookings = await _bookingCollection.Find(x => true).ToListAsync();
            return bookings.Select(b => new ResultBookingDto
            {
                BookingId = b.BookingId,
                PnrNumber = b.PnrNumber,
                FlightId = b.FlightId,
                ContactName = b.ContactName,
                ContactEmail = b.ContactEmail,
                ContactPhone = b.ContactPhone,
                PassengerCount = b.Passengers?.Count ?? 0,
                TotalPrice = b.TotalPrice,
                BookingDate = b.BookingDate,
                Status = b.Status
            }).ToList();
        }

        // 6 haneli, benzersiz bir PNR kodu uret
        private async Task<string> GenerateUniquePnrAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string pnr;
            bool exists;

            do
            {
                pnr = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
                exists = await _bookingCollection.Find(x => x.PnrNumber == pnr).AnyAsync();
            }
            while (exists);

            return pnr;
        }
    }
}
