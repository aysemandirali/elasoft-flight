using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    // Kullanıcının para birimi ve dil tercihini çerezde saklar.
    public class PreferencesController : Controller
    {
        private static readonly string[] AllowedCurrencies = { "TRY", "USD", "EUR" };
        private static readonly string[] AllowedLanguages = { "tr", "en" };

        public IActionResult SetCurrency(string code, string? returnUrl)
        {
            if (AllowedCurrencies.Contains(code))
                Response.Cookies.Append("cur", code, new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1), Path = "/" });

            return LocalRedirect(SafeReturn(returnUrl));
        }

        public IActionResult SetLanguage(string code, string? returnUrl)
        {
            if (AllowedLanguages.Contains(code))
                Response.Cookies.Append("lang", code, new CookieOptions { Expires = DateTimeOffset.Now.AddYears(1), Path = "/" });

            return LocalRedirect(SafeReturn(returnUrl));
        }

        private string SafeReturn(string? returnUrl)
            => (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) ? returnUrl : "/";
    }
}
