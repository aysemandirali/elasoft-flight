using System.Globalization;
using FlightBooking.Dtos.FlightDtos;
using FlightBooking.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace FlightBooking.Services.EmailServices
{
    // MailKit ile e-posta gonderir. Gelistirmede Mailpit (localhost:1025) kullanilir.
    // Mailpit calismiyorsa hata yutulur; rezervasyon akisi asla bozulmaz.
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendBookingConfirmationAsync(Booking booking, GetFlightByIdDto? flight)
        {
            try
            {
                var host = _config["EmailSettings:Host"] ?? "localhost";
                var port = int.TryParse(_config["EmailSettings:Port"], out var p) ? p : 1025;
                var fromName = _config["EmailSettings:FromName"] ?? "Geair";
                var fromAddress = _config["EmailSettings:FromAddress"] ?? "no-reply@geair.com";

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(fromName, fromAddress));
                message.To.Add(new MailboxAddress(booking.ContactName, booking.ContactEmail));
                message.Subject = $"Rezervasyon Onayı — PNR: {booking.PnrNumber}";
                message.Body = new BodyBuilder { HtmlBody = BuildHtml(booking, flight) }.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(host, port, SecureSocketOptions.None);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("Rezervasyon e-postası gönderildi: {Pnr} -> {Email}", booking.PnrNumber, booking.ContactEmail);
            }
            catch (Exception ex)
            {
                // E-posta gönderilemese bile rezervasyon tamamlanmış sayılır.
                _logger.LogWarning(ex, "Rezervasyon e-postası gönderilemedi (Mailpit çalışıyor mu?): {Pnr}", booking.PnrNumber);
            }
        }

        private static string BuildHtml(Booking booking, GetFlightByIdDto? flight)
        {
            var tr = CultureInfo.GetCultureInfo("tr-TR");
            var route = flight != null
                ? $"{flight.DepartureAirportCode} → {flight.ArrivalAirportCode}"
                : "-";
            var when = flight != null ? flight.DepartureTime.ToString("dd.MM.yyyy HH:mm") : "-";
            var flightNo = flight?.FlightNumber ?? "-";

            var passengers = string.Join("", booking.Passengers.Select(p =>
                $"<tr><td style='padding:6px 10px;border-bottom:1px solid #eee;'>{p.Name} {p.Surname}</td>" +
                $"<td style='padding:6px 10px;border-bottom:1px solid #eee;'>{p.PassengerType}</td></tr>"));

            return $@"
<div style='font-family:Arial,sans-serif;max-width:560px;margin:auto;border:1px solid #eee;border-radius:12px;overflow:hidden;'>
  <div style='background:linear-gradient(120deg,#1d4ed8,#0ea5e9);color:#fff;padding:24px;text-align:center;'>
    <h2 style='margin:0;'>Rezervasyonunuz Onaylandı</h2>
    <p style='margin:6px 0 0;opacity:.9;'>Geair ile seyahat edeceğiniz için teşekkürler</p>
  </div>
  <div style='padding:24px;color:#334155;'>
    <p>Sayın <b>{booking.ContactName}</b>, rezervasyonunuz oluşturuldu. PNR kodunuz:</p>
    <div style='text-align:center;margin:16px 0;'>
      <span style='display:inline-block;background:#0f172a;color:#fff;font-size:22px;letter-spacing:4px;padding:10px 20px;border-radius:8px;'>{booking.PnrNumber}</span>
    </div>
    <table style='width:100%;border-collapse:collapse;font-size:14px;'>
      <tr><td style='padding:6px 10px;color:#64748b;'>Uçuş</td><td style='padding:6px 10px;text-align:right;'><b>{flightNo}</b></td></tr>
      <tr><td style='padding:6px 10px;color:#64748b;'>Rota</td><td style='padding:6px 10px;text-align:right;'>{route}</td></tr>
      <tr><td style='padding:6px 10px;color:#64748b;'>Kalkış</td><td style='padding:6px 10px;text-align:right;'>{when}</td></tr>
      <tr><td style='padding:6px 10px;color:#64748b;'>Toplam</td><td style='padding:6px 10px;text-align:right;'><b>{booking.TotalPrice.ToString("#,##0", tr)} ₺</b></td></tr>
    </table>
    <h4 style='margin:18px 0 6px;'>Yolcular</h4>
    <table style='width:100%;border-collapse:collapse;font-size:14px;'>{passengers}</table>
    <p style='margin-top:20px;font-size:13px;color:#64748b;'>Online check-in uçuştan 24 saat önce açılır. İyi yolculuklar dileriz.</p>
  </div>
</div>";
        }
    }
}
