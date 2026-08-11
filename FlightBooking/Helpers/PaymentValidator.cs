using System.Text.RegularExpressions;

namespace FlightBooking.Helpers
{
    // Odeme formundaki kart bilgilerinin bicimini dogrular.
    // Kart bilgisi hicbir yerde saklanmaz; yalnizca bicim kontrolu yapilir.
    public static class PaymentValidator
    {
        // Kart numarasi: bosluklar yok sayilir, 16 hane olmali.
        public static bool IsCardNumberValid(string? cardNumber)
        {
            var digits = OnlyDigits(cardNumber);
            return digits.Length == 16;
        }

        // CVV: 3 hane olmali.
        public static bool IsCvvValid(string? cvv)
        {
            var digits = OnlyDigits(cvv);
            return digits.Length == 3;
        }

        // Son kullanma tarihi: AA/YY bicimi, ay 01-12 arasi.
        public static bool IsExpiryValid(string? expiry)
        {
            return Regex.IsMatch(expiry ?? string.Empty, @"^(0[1-9]|1[0-2])\/\d{2}$");
        }

        // Ucu birden gecerliyse odeme kabul edilir.
        public static bool IsValid(string? cardNumber, string? expiry, string? cvv)
        {
            return IsCardNumberValid(cardNumber) && IsExpiryValid(expiry) && IsCvvValid(cvv);
        }

        private static string OnlyDigits(string? value)
        {
            return new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        }
    }
}
