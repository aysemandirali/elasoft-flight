using FlightBooking.Dtos.OverBookingDtos;

namespace FlightBooking.Services.MachineLearningServices
{
    // Iki ML modelini (talep + no-show) + overbooking onerisi + gelir hesabini
    // birlestiren tahmin servisi. Her slot icin bir tahmin satiri uretir.
    public class OverbookingForecastService
    {
        private readonly FlightDemandMlService _demandService;
        private readonly NoShowMlService _noShowService;

        private static readonly string[] Slots =
            { "Morning-1", "Morning-2", "Afternoon-1", "Evening-1", "Night-1" };

        public OverbookingForecastService(FlightDemandMlService demandService, NoShowMlService noShowService)
        {
            _demandService = demandService;
            _noShowService = noShowService;
        }

        // Kac kayitla egitildigi (veri var mi kontrolu icin)
        public int TrainedRecordCount => _demandService.TrainedRecordCount;

        // Belirli bir ay ve kapasite icin tum slotlarin tahminini uret.
        public List<OverbookingForecastResultDto> Forecast(int month, int capacity, decimal averagePrice)
        {
            var results = new List<OverbookingForecastResultDto>();

            foreach (var slot in Slots)
            {
                // 1) Talep: kac bilet satilir (ML)
                int demand = (int)Math.Round(_demandService.Predict(slot, month, capacity));
                demand = Math.Clamp(demand, 0, capacity);

                // 2) No-show: kac kisi gelmez (ML)
                int noShow = (int)Math.Round(_noShowService.Predict(slot, demand, capacity));
                if (noShow < 0) noShow = 0;

                // 3) Overbooking onerisi
                int recommendedMaxSale = capacity + noShow;
                int extraSeats = noShow;

                // 4) Risk seviyesi (no-show orani)
                double noShowRate = demand > 0 ? (double)noShow / demand * 100 : 0;
                string risk = noShowRate >= 7 ? "High" : noShowRate >= 5 ? "Medium" : "Low";

                // 5) Tahmini gelir (onerilen satis * ortalama fiyat)
                decimal revenue = recommendedMaxSale * averagePrice;

                results.Add(new OverbookingForecastResultDto
                {
                    FlightSlot = slot,
                    Capacity = capacity,
                    ForecastPassenger = demand,
                    PredictedNoShow = noShow,
                    RecommendedMaxSale = recommendedMaxSale,
                    ExtraSeatCount = extraSeats,
                    RiskLevel = risk,
                    EstimatedRevenue = revenue
                });
            }

            return results;
        }
    }
}
