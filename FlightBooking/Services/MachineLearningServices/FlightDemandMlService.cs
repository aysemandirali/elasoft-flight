using FlightBooking.Entities;
using FlightBooking.MachineLearningModels;
using FlightBooking.Settings;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using MongoDB.Driver;

namespace FlightBooking.Services.MachineLearningServices
{
    // ML.NET ile ucus talep (satilacak bilet) tahmini. Singleton; model bir kez egitilir.
    public class FlightDemandMlService
    {
        private readonly IMongoCollection<NoShowHistory> _collection;
        private readonly MLContext _mlContext = new MLContext(seed: 0);
        private readonly object _lock = new object();

        private ITransformer? _model;
        private int _trainedRecordCount;

        public FlightDemandMlService(IOptions<DatabaseSettings> options)
        {
            var settings = options.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<NoShowHistory>(settings.NoShowHistoryCollection);
        }

        public int TrainedRecordCount => _trainedRecordCount;

        public bool Train()
        {
            var history = _collection.Find(x => x.SoldTickets > 0).ToList();
            if (history.Count < 5) return false;

            var data = history.Select(x => new FlightDemandInput
            {
                FlightSlot = x.FlightSlot,
                Month = ParseMonth(x.FlightDate),
                Capacity = x.Capacity,
                SoldTickets = x.SoldTickets
            });

            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Categorical
                .OneHotEncoding("SlotEncoded", nameof(FlightDemandInput.FlightSlot))
                .Append(_mlContext.Transforms.Concatenate("Features",
                    "SlotEncoded", nameof(FlightDemandInput.Month), nameof(FlightDemandInput.Capacity)))
                .Append(_mlContext.Regression.Trainers.FastTree(
                    labelColumnName: nameof(FlightDemandInput.SoldTickets),
                    featureColumnName: "Features",
                    numberOfLeaves: 12,
                    numberOfTrees: 100,
                    minimumExampleCountPerLeaf: 3,
                    learningRate: 0.2));

            lock (_lock)
            {
                _model = pipeline.Fit(dataView);
                _trainedRecordCount = history.Count;
            }
            return true;
        }

        public float Predict(string flightSlot, int month, float capacity)
        {
            lock (_lock)
            {
                if (_model == null && !Train()) return 0f;

                var engine = _mlContext.Model.CreatePredictionEngine<FlightDemandInput, FlightDemandPrediction>(_model!);
                var result = engine.Predict(new FlightDemandInput
                {
                    FlightSlot = flightSlot,
                    Month = month,
                    Capacity = capacity
                });
                return result.PredictedDemand < 0 ? 0 : result.PredictedDemand;
            }
        }

        // "2026-07-18" -> 7
        private static float ParseMonth(string flightDate)
        {
            var parts = flightDate.Split('-');
            return parts.Length >= 2 && int.TryParse(parts[1], out var m) ? m : 1;
        }
    }
}
