# ✈️ AI Destekli Uçuş Rezervasyon ve Biletleme Sistemi

ASP.NET Core MVC ve MongoDB ile geliştirilmiş, yapay zeka destekli bir uçuş rezervasyon uygulaması. Müşteri tarafında uçuş arama ve bilet alma, yönetici tarafında ise tam bir yönetim paneli, makine öğrenmesi tabanlı tahminler ve bir AI seyahat asistanı içerir.

## 🚀 Özellikler

### Müşteri Tarafı (Public)
- **Ana sayfa:** Uçuş arama, kampanyalar, canlı uçuş durumu tablosu, hizmetler ve sık sorulan sorular
- **Uçuş arama:** Nereden/nereye filtreleriyle uygun uçuşları listeleme
- **Bilet alma:** Yolcu ve iletişim bilgisiyle rezervasyon oluşturma, benzersiz PNR kodu üretimi
- **Üyelik/Giriş:** Şifreli (hash'li) kullanıcı kaydı ve çerez tabanlı oturum

### Yönetici Paneli (Admin)
- **Uçuş yönetimi:** Ekleme, listeleme, düzenleme, silme, detay (tam CRUD)
- **Rezervasyon yönetimi:** Rezervasyon oluşturma ve listeleme
- **Online check-in:** PNR ile sorgulama, koltuk atama ve biniş kartı üretimi
- **Dashboard:** Toplam uçuş, rezervasyon, gelir ve veri özetleri
- **Yetkilendirme:** Admin sayfaları giriş yapmadan erişime kapalı (`[Authorize]`)

### Yapay Zeka
- **AI Seyahat Asistanı:** Kullanıcının mesajından şehri tespit eder, gerçek hava durumu servisini (Open-Meteo) çağırır ve LLM (Google Gemini) ile öneri oluşturur — araç kullanan (tool calling) bir agent yapısı
- **No-Show Tahmini (ML.NET):** Geçmiş verilerle eğitilen FastTree regresyon modeli; bir uçuşta kaç yolcunun gelmeyeceğini tahmin eder
- **Uçuş Talep Tahmini (ML.NET):** Slot ve mevsime göre kaç bilet satılacağını (doluluk) tahmin eden ikinci regresyon modeli
- **Overbooking Önerisi:** Slot bazlı no-show oranlarından risk seviyesi ve fazla satılabilir koltuk önerisi

## 🛠️ Kullanılan Teknolojiler

| Alan | Teknoloji |
|------|-----------|
| Backend | ASP.NET Core MVC (.NET 10) |
| Veritabanı | MongoDB (MongoDB.Driver) |
| Nesne Eşleme | AutoMapper |
| Makine Öğrenmesi | ML.NET (FastTree Regresyon) |
| Yapay Zeka | Google Gemini API |
| Hava Durumu | Open-Meteo (ücretsiz) |
| Kimlik Doğrulama | Cookie Authentication + PasswordHasher |
| Arayüz | Bootstrap 5, geair HTML şablonu |

## 📦 Kurulum

### Gereksinimler
- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [MongoDB Community Server](https://www.mongodb.com/try/download/community) (yerel olarak `localhost:27017`)

### Adımlar

1. Projeyi klonlayın:
   ```bash
   git clone https://github.com/aysemandirali/elasoft-flight.git
   cd elasoft-flight/FlightBooking
   ```

2. MongoDB'nin çalıştığından emin olun (varsayılan: `mongodb://localhost:27017`).

3. **(AI asistanı için opsiyonel)** `FlightBooking` klasörüne `appsettings.Local.json` dosyası ekleyin ve Google Gemini API anahtarınızı girin:
   ```json
   {
     "Gemini": {
       "ApiKey": "BURAYA_KENDI_ANAHTARINIZ",
       "Model": "gemini-flash-latest"
     }
   }
   ```
   > Bu dosya `.gitignore`'da olduğundan anahtarınız repoya gitmez.

4. Uygulamayı çalıştırın:
   ```bash
   dotnet run
   ```

5. Tarayıcıdan açın: `http://localhost:5199`

### İlk Kullanım
- Admin paneline erişmek için önce ana sayfadan **Üye Ol** ile bir hesap açın, ardından **Giriş Yap**.
- Overbooking ve ML tahmin sayfalarındaki **"Örnek Veri Yükle"** butonuyla demo verisini oluşturabilirsiniz.

## 📁 Proje Yapısı

```
FlightBooking/
├── Areas/Admin/        # Yönetici paneli (controller, view, component)
├── Controllers/        # Public controller'lar (Default, Flight, Agent, Account)
├── Entities/           # MongoDB varlıkları (Flight, Booking, Passenger, ...)
├── Dtos/               # Veri transfer nesneleri
├── Services/           # İş katmanı servisleri (Flight, Booking, CheckIn, ML, Auth)
├── AgentServices/      # AI agent (Gemini, WeatherTool, TravelAgent)
├── MachineLearningModels/  # ML.NET giriş/çıkış modelleri
├── Mapping/            # AutoMapper profilleri
├── Settings/           # Veritabanı ve API ayarları
└── wwwroot/geair/      # Arayüz şablonu ve statik dosyalar
```

## 📝 Notlar
- Bu proje eğitim amacıyla, adım adım geliştirilmiştir.
- MongoDB koleksiyonları (`Flights`, `Bookings`, `NoShowHistories`, `Users`) uygulama çalışırken otomatik oluşur.
