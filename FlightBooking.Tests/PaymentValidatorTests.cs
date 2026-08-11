using FlightBooking.Helpers;

namespace FlightBooking.Tests
{
    // Odeme ekranindaki kart bilgisi dogrulamasi.
    public class PaymentValidatorTests
    {
        [Theory]
        [InlineData("4242424242424242")]      // bosluksuz 16 hane
        [InlineData("4242 4242 4242 4242")]   // bosluklu yazim da kabul edilmeli
        public void GecerliKartNumarasi_KabulEdilir(string cardNumber)
        {
            Assert.True(PaymentValidator.IsCardNumberValid(cardNumber));
        }

        [Theory]
        [InlineData("12345")]                 // eksik hane
        [InlineData("42424242424242421")]     // fazla hane
        [InlineData("")]                      // bos
        [InlineData(null)]                    // hic girilmemis
        [InlineData("abcd efgh ijkl mnop")]   // rakam yok
        public void GecersizKartNumarasi_Reddedilir(string? cardNumber)
        {
            Assert.False(PaymentValidator.IsCardNumberValid(cardNumber));
        }

        [Theory]
        [InlineData("01/28")]
        [InlineData("12/30")]
        public void GecerliSonKullanmaTarihi_KabulEdilir(string expiry)
        {
            Assert.True(PaymentValidator.IsExpiryValid(expiry));
        }

        [Theory]
        [InlineData("13/28")]   // ay 12'den buyuk olamaz
        [InlineData("00/28")]   // ay sifir olamaz
        [InlineData("1228")]    // ayirac yok
        [InlineData("12/2028")] // yil dort haneli
        [InlineData("")]
        public void GecersizSonKullanmaTarihi_Reddedilir(string expiry)
        {
            Assert.False(PaymentValidator.IsExpiryValid(expiry));
        }

        [Fact]
        public void UcHaneliCvv_KabulEdilir()
        {
            Assert.True(PaymentValidator.IsCvvValid("123"));
        }

        [Theory]
        [InlineData("12")]     // eksik
        [InlineData("1234")]   // fazla
        [InlineData("abc")]    // rakam degil
        public void GecersizCvv_Reddedilir(string cvv)
        {
            Assert.False(PaymentValidator.IsCvvValid(cvv));
        }

        [Fact]
        public void TumBilgilerDogruysa_OdemeGecerlidir()
        {
            Assert.True(PaymentValidator.IsValid("4242 4242 4242 4242", "12/28", "123"));
        }

        [Fact]
        public void TekBirAlanHataliysa_OdemeGecersizdir()
        {
            // kart ve tarih dogru, yalnizca CVV hatali
            Assert.False(PaymentValidator.IsValid("4242424242424242", "12/28", "12"));
        }
    }
}
