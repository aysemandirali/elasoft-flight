using FlightBooking.Helpers;
using Microsoft.AspNetCore.Http;

namespace FlightBooking.Tests
{
    // Fiyatlarin secilen para birimine gore gosterimi.
    // Temel para birimi TL'dir; USD ve EUR sabit kurla cevrilir.
    public class MoneyHelperTests
    {
        // Secili para birimini cerezle tasiyan sahte bir istek olusturur.
        private static HttpContext ContextWithCurrency(string? currency)
        {
            var ctx = new DefaultHttpContext();
            if (currency != null)
                ctx.Request.Headers["Cookie"] = $"cur={currency}";
            return ctx;
        }

        [Fact]
        public void SecimYapilmamissa_VarsayilanParaBirimiTLdir()
        {
            Assert.Equal("TRY", MoneyHelper.CurrentCurrency(ContextWithCurrency(null)));
        }

        [Theory]
        [InlineData("USD", "USD")]
        [InlineData("EUR", "EUR")]
        [InlineData("TRY", "TRY")]
        public void SecilenParaBirimi_Okunur(string cerez, string beklenen)
        {
            Assert.Equal(beklenen, MoneyHelper.CurrentCurrency(ContextWithCurrency(cerez)));
        }

        [Fact]
        public void TanimsizParaBirimi_TLyeDuser()
        {
            Assert.Equal("TRY", MoneyHelper.CurrentCurrency(ContextWithCurrency("GBP")));
        }

        [Fact]
        public void TLTutari_SembolIleSonaYazilir()
        {
            var sonuc = MoneyHelper.Format(ContextWithCurrency("TRY"), 6466m);
            Assert.Contains("₺", sonuc);
            Assert.Contains("6.466", sonuc); // binlik ayraci nokta
        }

        [Fact]
        public void DolarSecildiginde_TutarBolunurVeBasaSembolGelir()
        {
            // 3200 TL / 32 = 100 USD
            var sonuc = MoneyHelper.Format(ContextWithCurrency("USD"), 3200m);
            Assert.StartsWith("$", sonuc);
            Assert.Contains("100", sonuc);
        }

        [Fact]
        public void EuroSecildiginde_TutarBolunurVeBasaSembolGelir()
        {
            // 3500 TL / 35 = 100 EUR
            var sonuc = MoneyHelper.Format(ContextWithCurrency("EUR"), 3500m);
            Assert.StartsWith("€", sonuc);
            Assert.Contains("100", sonuc);
        }

        [Fact]
        public void TLdeKurUygulanmaz()
        {
            Assert.Equal(1m, MoneyHelper.Rate(ContextWithCurrency("TRY")));
        }

        [Fact]
        public void YabanciParaBirimlerinde_SembolBasaGelir()
        {
            Assert.True(MoneyHelper.SymbolPrefix(ContextWithCurrency("USD")));
            Assert.False(MoneyHelper.SymbolPrefix(ContextWithCurrency("TRY")));
        }
    }
}
