using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Driver;

namespace FlightBooking.Services.CheckInServices
{
    public class CheckInService : ICheckInService
    {
        private readonly IMongoCollection<Booking> _bookingCollection;

        public CheckInService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _bookingCollection = database.GetCollection<Booking>(settings.BookingCollectionName);
        }

        public async Task<Booking?> GetBookingByPnrAsync(string pnr)
        {
            return await _bookingCollection.Find(x => x.PnrNumber == pnr).FirstOrDefaultAsync();
        }

        public async Task CheckInPassengerAsync(string pnr, int passengerIndex, string seatNumber)
        {
            var booking = await GetBookingByPnrAsync(pnr);
            if (booking == null) return;
            if (passengerIndex < 0 || passengerIndex >= booking.Passengers.Count) return;

            var passenger = booking.Passengers[passengerIndex];
            passenger.IsCheckedIn = true;
            passenger.CheckInStatus = "Checked-In";
            passenger.CheckInDate = DateTime.Now;
            passenger.SeatNumber = seatNumber;
            passenger.TicketStatus = "Issued";
            // Basit bir biniş kartı numarası üret
            passenger.BoardingPassNumber = "BP-" + pnr + "-" + (passengerIndex + 1);

            // Guncellenmis rezervasyonu geri yaz
            await _bookingCollection.ReplaceOneAsync(x => x.BookingId == booking.BookingId, booking);
        }
    }
}
