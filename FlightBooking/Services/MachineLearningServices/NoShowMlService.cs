using FlightBooking.Entities;
using FlightBooking.MachineLearningModels;
using FlightBooking.Settings;
using Microsoft.Extensions.Options;
using Microsoft.ML;
using MongoDB.Driver;

namespace FlightBooking.Services.MachineLearningServices
{
    // ML.NET ile no-show tahmini. Singleton olarak tutulur; model bir kez egitilir.
    public class NoShowMlService
    {
        private readonly IMongoCollection<NoShowHistory> _collection;
        private readonly MLContext _mlContext = new MLContext(seed: 0);
        private readonly object _lock = new object();

        private ITransformer? _model;
        private int _trainedRecordCount;

        public NoShowMlService(IOptions<DatabaseSettings> options)
        {
            var settings = options.Value;
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _collection = database.GetCollection<NoShowHistory>(settings.NoShowHistoryCollection);
        }

        // Kac kayitla egitildigi (UI'da gostermek icin)
        public int TrainedRecordCount => _trainedRecordCount;

        // Modeli gecmis veriyle egit (yeterli veri varsa true doner)
        public bool Train()
        {
            var history = _collection.Find(x => x.SoldTickets > 0).ToList();
            if (history.Count < 5) return false; // egitim icin cok az veri

            var data = history.Select(x => new NoShowMlInput
            {
                FlightSlot = x.FlightSlot,
                SoldTickets = x.SoldTickets,
                Capacity = x.Capacity,
                NoShowPassenger = x.NoShowPassenger
            });

            var dataView = _mlContext.Data.LoadFromEnumerable(data);

            var pipeline = _mlContext.Transforms.Categorical
                .OneHotEncoding("SlotEncoded", nameof(NoShowMlInput.FlightSlot))
                .Append(_mlContext.Transforms.Concatenate("Features",
                    "SlotEncoded", nameof(NoShowMlInput.SoldTickets), nameof(NoShowMlInput.Capacity)))
                .Append(_mlContext.Regression.Trainers.FastTree(
                    labelColumnName: nameof(NoShowMlInput.NoShowPassenger),
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

        // Verilen ozelliklere gore no-show tahmini yap
        public float Predict(string flightSlot, float soldTickets, float capacity)
        {
            lock (_lock)
            {
                if (_model == null && !Train()) return 0f;

                var engine = _mlContext.Model.CreatePredictionEngine<NoShowMlInput, NoShowMlPrediction>(_model!);
                var result = engine.Predict(new NoShowMlInput
                {
                    FlightSlot = flightSlot,
                    SoldTickets = soldTickets,
                    Capacity = capacity
                });
                // Negatif tahmini 0'a cekelim
                return result.PredictedNoShow < 0 ? 0 : result.PredictedNoShow;
            }
        }
    }
}
