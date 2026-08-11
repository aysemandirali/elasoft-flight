using AutoMapper;
using FlightBooking.Dtos.FlightDtos;
using FlightBooking.Entities;
using FlightBooking.Settings;
using MongoDB.Driver;

namespace FlightBooking.Services.FlightServices
{
    public class FlightService : IFlightService
    {
        private readonly IMapper _mapper;
        private readonly IMongoCollection<Flight> _flightCollection;

        public FlightService(IMapper mapper, IDatabaseSettings databaseSettings)
        {
            // MongoDB baglantisini kur ve "Flights" koleksiyonunu al.
            var client = new MongoClient(databaseSettings.ConnectionString);
            var database = client.GetDatabase(databaseSettings.DatabaseName);
            _flightCollection = database.GetCollection<Flight>(databaseSettings.FlightCollectionName);
            _mapper = mapper;
        }

        public async Task CreateFlightAsync(CreateFlightDto createFlightDto)
        {
            var value = _mapper.Map<Flight>(createFlightDto);
            await _flightCollection.InsertOneAsync(value);
        }

        public async Task DeleteFlightAsync(string id)
        {
            await _flightCollection.DeleteOneAsync(x => x.FlightId == id);
        }

        public async Task<List<ResultFlightDto>> GetAllFlightsAsync()
        {
            var values = await _flightCollection.Find(x => true).ToListAsync();
            return _mapper.Map<List<ResultFlightDto>>(values);
        }

        public async Task<GetFlightByIdDto> GetFlightByIdAsync(string id)
        {
            // Adres cubugundan bozuk bir kimlik gelirse sorgu calistirilmadan bos donulur.
            if (!MongoDB.Bson.ObjectId.TryParse(id, out _))
                return null!;

            var value = await _flightCollection.Find(x => x.FlightId == id).FirstOrDefaultAsync();
            return _mapper.Map<GetFlightByIdDto>(value);
        }

        public async Task UpdateFlightAsync(UpdateFlightDto updateFlightDto)
        {
            var value = _mapper.Map<Flight>(updateFlightDto);
            await _flightCollection.FindOneAndReplaceAsync(x => x.FlightId == updateFlightDto.FlightId, value);
        }
    }
}
