using FlightBooking.Dtos.BookingDtos;
using FlightBooking.Entities;
using FlightBooking.Services.BookingServices;
using FlightBooking.Services.CheckInServices;
using FlightBooking.Services.EmailServices;
using FlightBooking.Services.FlightServices;
using Microsoft.AspNetCore.Mvc;

namespace FlightBooking.Controllers
{
    // Halka acik (musteri) ucus arama, bilet alma, seyahat sorgulama ve check-in.
    public class FlightController : Controller
    {
        private readonly IFlightService _flightService;
        private readonly IBookingService _bookingService;
        private readonly ICheckInService _checkInService;
        private readonly IEmailService _emailService;

        public FlightController(IFlightService flightService, IBookingService bookingService, ICheckInService checkInService, IEmailService emailService)
        {
            _flightService = flightService;
            _bookingService = bookingService;
            _checkInService = checkInService;
            _emailService = emailService;
        }

        // Ucus listesi + basit arama (kalkis/varis koduna gore)
        [HttpGet]
        public async Task<IActionResult> Index(string? from, string? to)
        {
            var flights = await _flightService.GetAllFlightsAsync();

            // Sadece bos koltugu olan ucuslar
            flights = flights.Where(x => x.AvailableSeats > 0).ToList();

            if (!string.IsNullOrWhiteSpace(from))
                flights = flights.Where(x => x.DepartureAirportCode.Contains(from.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();
            if (!string.IsNullOrWhiteSpace(to))
                flights = flights.Where(x => x.ArrivalAirportCode.Contains(to.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            ViewBag.From = from;
            ViewBag.To = to;
            return View(flights);
        }

        // Secilen ucus icin bilet alma formu
        [HttpGet]
        public async Task<IActionResult> Book(string id)
        {
            var flight = await _flightService.GetFlightByIdAsync(id);

            // Uçuş bulunamazsa (bozuk veya eski bağlantı) listeye geri dön
            if (flight == null) return RedirectToAction("Index");

            // Giriş yapan müşterinin bilgilerini forma önceden doldur
            // (böylece rezervasyon otomatik olarak hesabına bağlanır)
            if (User.Identity?.IsAuthenticated == true)
            {
                ViewBag.UserName = User.Identity.Name;
                ViewBag.UserEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            }

            return View(flight);
        }

        // Bilet alma formunu kaydet, odeme ekranina gec
        [HttpPost]
        public async Task<IActionResult> Book(CreateBookingDto dto)
        {
            var pnr = await _bookingService.CreateBookingAsync(dto);

            // Rezervasyon onay e-postasını gönder (hata olsa da akış devam eder)
            var booking = await _bookingService.GetByPnrAsync(pnr);
            if (booking != null)
            {
                var flight = await _flightService.GetFlightByIdAsync(booking.FlightId);
                await _emailService.SendBookingConfirmationAsync(booking, flight);
            }

            return RedirectToAction("Payment", new { pnr });
        }

        // Odeme ekrani: rezervasyon ozeti + kart formu
        [HttpGet]
        public async Task<IActionResult> Payment(string pnr)
        {
            var booking = await _bookingService.GetByPnrAsync(pnr);
            if (booking == null) return RedirectToAction("Index");

            // Zaten odenmisse tekrar odeme alma
            if (booking.PaymentStatus == "Ödendi")
                return RedirectToAction("Confirmation", new { pnr });

            ViewBag.Flight = await _flightService.GetFlightByIdAsync(booking.FlightId);
            return View(booking);
        }

        // Odemeyi tamamla (simulasyon): kart bilgileri dogru formatta ise odendi say.
        // Sunucu tarafi dogrulama; kart bilgisi saklanmaz, sadece format kontrol edilir.
        [HttpPost]
        public async Task<IActionResult> PaymentComplete(string pnr, string? cardNumber, string? cardExpiry, string? cardCvv)
        {
            var booking = await _bookingService.GetByPnrAsync(pnr);
            if (booking == null) return RedirectToAction("Index");

            if (!FlightBooking.Helpers.PaymentValidator.IsValid(cardNumber, cardExpiry, cardCvv))
            {
                TempData["PayError"] = "Kart bilgileri geçersiz. Kart numarası 16 hane, CVV 3 hane ve son kullanma AA/YY biçiminde olmalı.";
                return RedirectToAction("Payment", new { pnr });
            }

            await _bookingService.MarkAsPaidAsync(pnr);
            return RedirectToAction("Confirmation", new { pnr });
        }

        // Onay sayfasi (PNR gosterir)
        [HttpGet]
        // Rezervasyon sorgulama (Seyahatlerim): PNR + soyad ile rezervasyonu göster
        [HttpGet]
        public async Task<IActionResult> MyBooking(string? pnr, string? surname)
        {
            ViewBag.Searched = !string.IsNullOrWhiteSpace(pnr);
            ViewBag.Pnr = pnr;
            ViewBag.Surname = surname;

            if (string.IsNullOrWhiteSpace(pnr))
                return View((Booking?)null);

            var booking = await _bookingService.GetByPnrAsync(pnr.Trim().ToUpper());

            // Soyad girildiyse doğrula
            if (booking != null && !string.IsNullOrWhiteSpace(surname) &&
                !booking.Passengers.Any(p => p.Surname.Equals(surname.Trim(), StringComparison.OrdinalIgnoreCase)))
                booking = null;

            ViewBag.Flight = booking != null ? await _flightService.GetFlightByIdAsync(booking.FlightId) : null;
            return View(booking);
        }

        // Müşteri online check-in: PNR + soyad ile rezervasyonu getir, yolcuları göster
        [HttpGet]
        public async Task<IActionResult> CheckIn(string? pnr, string? surname)
        {
            ViewBag.Searched = !string.IsNullOrWhiteSpace(pnr);
            ViewBag.Pnr = pnr;
            ViewBag.Surname = surname;

            if (string.IsNullOrWhiteSpace(pnr))
                return View((Booking?)null);

            var booking = await _bookingService.GetByPnrAsync(pnr.Trim().ToUpper());
            if (booking != null && !string.IsNullOrWhiteSpace(surname) &&
                !booking.Passengers.Any(p => p.Surname.Equals(surname.Trim(), StringComparison.OrdinalIgnoreCase)))
            {
                booking = null;
                ViewBag.NotFound = true;
            }

            // Görsel koltuk haritası için: bu uçuştaki tüm dolu koltuklar
            if (booking != null)
            {
                var allBookings = await _bookingService.GetAllRawAsync();
                var occupied = allBookings
                    .Where(b => b.FlightId == booking.FlightId)
                    .SelectMany(b => b.Passengers)
                    .Where(p => !string.IsNullOrWhiteSpace(p.SeatNumber))
                    .Select(p => p.SeatNumber!)
                    .Distinct()
                    .ToList();
                ViewBag.OccupiedSeats = occupied;
            }

            return View(booking);
        }

        // Müşteri check-in'i tamamla: ek hizmet ücreti varsa önce ödeme ekranına,
        // yoksa doğrudan check-in'i uygula.
        // Bir koltuk, aynı uçuşta başka bir yolcuya atanmış mı? (kendisi hariç)
        private async Task<bool> IsSeatTakenAsync(Booking booking, int passengerIndex, string seat)
        {
            if (string.IsNullOrWhiteSpace(seat)) return false;
            var all = await _bookingService.GetAllRawAsync();
            foreach (var b in all.Where(x => x.FlightId == booking.FlightId))
                for (int i = 0; i < b.Passengers.Count; i++)
                {
                    if (b.PnrNumber == booking.PnrNumber && i == passengerIndex) continue; // yolcunun kendisi
                    if (string.Equals(b.Passengers[i].SeatNumber, seat, StringComparison.OrdinalIgnoreCase))
                        return true;
                }
            return false;
        }

        [HttpPost]
        public async Task<IActionResult> CheckInComplete(string pnr, int passengerIndex, string seatNumber,
                                                         int extraBaggageKg, string? mealType, bool seatUpgrade)
        {
            var target = await _bookingService.GetByPnrAsync(pnr);
            if (target == null) return RedirectToAction("CheckIn", new { pnr });

            // Koltuk başka yolcuya aitse check-in'i engelle
            if (await IsSeatTakenAsync(target, passengerIndex, seatNumber))
            {
                TempData["CheckInError"] = $"{seatNumber} koltuğu dolu. Lütfen başka bir koltuk seçin.";
                return RedirectToAction("CheckIn", new { pnr });
            }

            var extraCost = _checkInService.CalculateExtraCost(extraBaggageKg, mealType, seatUpgrade);

            if (extraCost > 0)
            {
                // Ücretli ek hizmet seçildi -> ödeme adımına yönlendir
                return RedirectToAction("CheckInPayment", new { pnr, passengerIndex, seatNumber, extraBaggageKg, mealType, seatUpgrade });
            }

            await _checkInService.CheckInPassengerAsync(pnr, passengerIndex, seatNumber, extraBaggageKg, mealType, seatUpgrade);
            return RedirectToAction("CheckIn", new { pnr });
        }

        // Ek hizmet ödeme ekranı (bagaj/yemek/koltuk ücreti için)
        [HttpGet]
        public async Task<IActionResult> CheckInPayment(string pnr, int passengerIndex, string seatNumber,
                                                        int extraBaggageKg, string? mealType, bool seatUpgrade)
        {
            var booking = await _bookingService.GetByPnrAsync(pnr);
            if (booking == null || passengerIndex < 0 || passengerIndex >= booking.Passengers.Count)
                return RedirectToAction("CheckIn", new { pnr });

            var extraCost = _checkInService.CalculateExtraCost(extraBaggageKg, mealType, seatUpgrade);

            var p = booking.Passengers[passengerIndex];
            ViewBag.PassengerName = $"{p.Name} {p.Surname}";
            ViewBag.Pnr = pnr;
            ViewBag.PassengerIndex = passengerIndex;
            ViewBag.SeatNumber = seatNumber;
            ViewBag.ExtraBaggageKg = extraBaggageKg;
            ViewBag.MealType = mealType;
            ViewBag.SeatUpgrade = seatUpgrade;
            ViewBag.ExtraCost = extraCost;

            return View();
        }

        // Ek hizmet ödemesini tamamla -> check-in'i uygula
        [HttpPost]
        public async Task<IActionResult> CheckInPaymentComplete(string pnr, int passengerIndex, string seatNumber,
                                                               int extraBaggageKg, string? mealType, bool seatUpgrade,
                                                               string? cardNumber, string? cardExpiry, string? cardCvv)
        {
            if (!FlightBooking.Helpers.PaymentValidator.IsValid(cardNumber, cardExpiry, cardCvv))
            {
                TempData["PayError"] = "Kart bilgileri geçersiz. Kart numarası 16 hane, CVV 3 hane ve son kullanma AA/YY biçiminde olmalı.";
                return RedirectToAction("CheckInPayment", new { pnr, passengerIndex, seatNumber, extraBaggageKg, mealType, seatUpgrade });
            }

            // Ödeme sırasında koltuk başkası tarafından alındıysa engelle
            var target = await _bookingService.GetByPnrAsync(pnr);
            if (target != null && await IsSeatTakenAsync(target, passengerIndex, seatNumber))
            {
                TempData["CheckInError"] = $"{seatNumber} koltuğu bu sırada dolduğu için işlem tamamlanamadı. Lütfen başka bir koltuk seçin.";
                return RedirectToAction("CheckIn", new { pnr });
            }

            await _checkInService.CheckInPassengerAsync(pnr, passengerIndex, seatNumber, extraBaggageKg, mealType, seatUpgrade);
            TempData["CheckInOk"] = "Ödemeniz alındı ve check-in tamamlandı.";
            return RedirectToAction("CheckIn", new { pnr });
        }

        // Müşteri kendi rezervasyonunu iptal eder (PNR ile)
        [HttpPost]
        public async Task<IActionResult> CancelBooking(string pnr)
        {
            var booking = await _bookingService.GetByPnrAsync(pnr);
            if (booking != null && booking.Status != "Cancelled")
                await _bookingService.CancelBookingAsync(pnr);

            return RedirectToAction("MyBooking", new { pnr });
        }

        // Uçuş durumu sorgulama: uçuş numarasına göre uçuşları ve durumlarını göster
        [HttpGet]
        public async Task<IActionResult> FlightStatus(string? flightNumber)
        {
            ViewBag.Searched = !string.IsNullOrWhiteSpace(flightNumber);
            ViewBag.FlightNumber = flightNumber;

            var all = await _flightService.GetAllFlightsAsync();
            var results = string.IsNullOrWhiteSpace(flightNumber)
                ? all.OrderBy(x => x.DepartureTime).Take(10).ToList()
                : all.Where(x => x.FlightNumber.Contains(flightNumber.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

            return View(results);
        }

        public async Task<IActionResult> Confirmation(string? pnr)
        {
            pnr ??= TempData["Pnr"] as string;
            ViewBag.Pnr = pnr;

            if (!string.IsNullOrWhiteSpace(pnr))
            {
                var booking = await _bookingService.GetByPnrAsync(pnr);
                ViewBag.Paid = booking?.PaymentStatus == "Ödendi";
            }
            return View();
        }
    }
}
