using FlightBooking.AgentServices.IntentDetectors;

namespace FlightBooking.Tests
{
    // AI asistaninin, kullanicinin mesajindan niyeti anlamasi.
    public class TravelIntentDetectorTests
    {
        private readonly IIntentDetector _detector = new TravelIntentDetector();

        [Theory]
        [InlineData("Antalya'da nerede yemek yenir?")]
        [InlineData("İyi bir restoran önerir misin?")]
        public void YemekIceren_MesajRestoranNiyeti(string mesaj)
        {
            Assert.Equal(TravelIntent.Restaurant, _detector.Detect(mesaj));
        }

        [Theory]
        [InlineData("Yarın hava nasıl olacak?")]
        [InlineData("İzmir kaç derece?")]
        public void HavaDurumuIceren_MesajWeatherNiyeti(string mesaj)
        {
            Assert.Equal(TravelIntent.Weather, _detector.Detect(mesaj));
        }

        [Theory]
        [InlineData("Roma'da uygun bir otel var mı?")]
        [InlineData("Nerede kalacak yer bulabilirim?")]
        public void KonaklamaIceren_MesajHotelNiyeti(string mesaj)
        {
            Assert.Equal(TravelIntent.Hotel, _detector.Detect(mesaj));
        }

        [Theory]
        [InlineData("Paris'te gezilecek yerler neler?")]
        [InlineData("Hangi müze görülmeli?")]
        public void GeziIceren_MesajAttractionNiyeti(string mesaj)
        {
            Assert.Equal(TravelIntent.Attraction, _detector.Detect(mesaj));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void BosMesaj_NiyetBelirlenemez(string? mesaj)
        {
            Assert.Equal(TravelIntent.Unknown, _detector.Detect(mesaj!));
        }

        [Fact]
        public void BuyukKucukHarf_FarkYaratmaz()
        {
            Assert.Equal(TravelIntent.Weather, _detector.Detect("HAVA nasıl?"));
        }
    }
}
