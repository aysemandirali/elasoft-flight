using FlightBooking.Services.CheckInServices;
using FlightBooking.Settings;

namespace FlightBooking.Tests
{
    // Check-in sirasinda secilen ek hizmetlerin ucret hesabi.
    // Fiyatlar: bagaj kg basina 15 TL, yemek tipe gore, on sira koltuk 100 TL.
    public class CheckInExtraCostTests
    {
        private static ICheckInService CreateService()
        {
            // Ucret hesabi veritabanina gitmez; baglanti yalnizca nesne olusturmak icin verilir.
            var settings = new DatabaseSettings
            {
                ConnectionString = "mongodb://localhost:27017",
                DatabaseName = "FlightBookingTestDb",
                BookingCollectionName = "Bookings",
                CheckInCollectionName = "CheckIns"
            };
            return new CheckInService(settings);
        }

        [Fact]
        public void HicEkHizmetSecilmezse_UcretSifirdir()
        {
            var service = CreateService();
            Assert.Equal(0m, service.CalculateExtraCost(0, "Yok", false));
        }

        [Theory]
        [InlineData(10, 150)]   // 10 kg * 15 TL
        [InlineData(20, 300)]   // 20 kg * 15 TL
        public void EkBagaj_KiloBasina15TlHesaplanir(int kg, decimal beklenen)
        {
            var service = CreateService();
            Assert.Equal(beklenen, service.CalculateExtraCost(kg, "Yok", false));
        }

        [Theory]
        [InlineData("Standart", 50)]
        [InlineData("Vejetaryen", 80)]
        [InlineData("Premium Menü", 150)]
        public void YemekSecimi_TipineGoreUcretlendirilir(string yemek, decimal beklenen)
        {
            var service = CreateService();
            Assert.Equal(beklenen, service.CalculateExtraCost(0, yemek, false));
        }

        [Fact]
        public void KoltukYukseltmesi_100TlEkler()
        {
            var service = CreateService();
            Assert.Equal(100m, service.CalculateExtraCost(0, "Yok", true));
        }

        [Fact]
        public void BirdenFazlaHizmet_ToplanarakHesaplanir()
        {
            var service = CreateService();
            // 10 kg bagaj (150) + Standart yemek (50) + koltuk yukseltme (100)
            Assert.Equal(300m, service.CalculateExtraCost(10, "Standart", true));
        }

        [Fact]
        public void TanimsizYemekTipi_UcretEklemez()
        {
            var service = CreateService();
            Assert.Equal(0m, service.CalculateExtraCost(0, "Bilinmeyen", false));
        }
    }
}
