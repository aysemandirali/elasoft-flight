using System.Globalization;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlightBooking.Helpers
{
    // Fiyatları, kullanıcının seçtiği para birimine (çerez) göre biçimlendirir.
    // Temel para birimi TRY'dir; sabit kurlarla USD/EUR'a çevrilir.
    public static class MoneyHelper
    {
        // 1 birim yabancı para kaç TL (yaklaşık, sabit).
        private const decimal UsdRate = 32m;
        private const decimal EurRate = 35m;

        public static string CurrentCurrency(HttpContext ctx)
            => ctx.Request.Cookies["cur"] switch { "USD" => "USD", "EUR" => "EUR", _ => "TRY" };

        // Canlı (JS) fiyat hesabı yapan sayfalar için kur/sembol bilgileri.
        public static decimal Rate(HttpContext ctx)
            => CurrentCurrency(ctx) switch { "USD" => UsdRate, "EUR" => EurRate, _ => 1m };

        public static string Symbol(HttpContext ctx)
            => CurrentCurrency(ctx) switch { "USD" => "$", "EUR" => "€", _ => "₺" };

        public static bool SymbolPrefix(HttpContext ctx) => CurrentCurrency(ctx) != "TRY";

        // Ham TL tutarını seçili para birimine çevirip metin olarak döndürür.
        public static string Format(HttpContext ctx, decimal tl)
        {
            var cur = CurrentCurrency(ctx);
            var trFmt = CultureInfo.GetCultureInfo("tr-TR");
            var enFmt = CultureInfo.GetCultureInfo("en-US");

            return cur switch
            {
                "USD" => "$" + (tl / UsdRate).ToString("#,##0", enFmt),
                "EUR" => "€" + (tl / EurRate).ToString("#,##0", enFmt),
                _ => tl.ToString("#,##0", trFmt) + " ₺"
            };
        }

        // View'larda kısa kullanım: @Html.Money(fiyat)
        public static IHtmlContent Money(this IHtmlHelper html, decimal tl)
            => new HtmlString(Format(html.ViewContext.HttpContext, tl));
    }
}
