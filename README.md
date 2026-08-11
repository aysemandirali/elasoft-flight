# ✈️ AI Destekli Uçuş Rezervasyon ve Biletleme Sistemi

ASP.NET Core MVC ve MongoDB ile geliştirilmiş, yapay zeka destekli bir seyahat platformu. Müşteri tarafında uçuş arama, ödemeli bilet satın alma, görsel koltuk seçimli online check-in ve araç kiralama / tur / transfer modülleri; yönetici tarafında ise ayrı girişli tam bir yönetim paneli, makine öğrenmesi tabanlı tahminler ve bir AI seyahat asistanı bulunur.

## 🚀 Özellikler

### Müşteri Tarafı (giriş gerektirmez)
- **Ana sayfa:** Uçuş arama, kampanyalar, canlı uçuş durumu, hizmetler ve sık sorulan sorular
- **Uçuş arama ve bilet alma:** Nereden/nereye filtresi, yolcu bilgileriyle rezervasyon, benzersiz PNR üretimi
- **Ödeme ekranı:** Kart önizlemeli ödeme formu, sunucu tarafı kart doğrulaması (16 hane / CVV / AA-YY), ödeme durumu takibi *(demo — gerçek tahsilat yapılmaz)*
- **Online check-in:** Uçağın kabin düzenini gösteren **görsel koltuk haritası** (dolu koltuklar seçilemez, çift atama engellenir), biniş kartı üretimi
- **Ek hizmetler:** Check-in sırasında bagaj, yemek ve koltuk yükseltmesi seçimi — ücretli hizmetler için ayrı ödeme adımı
- **Seyahatlerim / Hesabım:** PNR ile rezervasyon sorgulama, ödeme durumu, **rezervasyon iptali**
- **Uçuş durumu:** Uçuş numarasına göre anlık durum sorgulama
- **Araç kiralama:** Şehir/sınıf/tarih ile arama, gün bazlı canlı fiyat, rezervasyon kodu ile onay
- **Turlar & aktiviteler:** Şehir ve türe göre arama, kişi sayısına göre fiyat, rezervasyon
- **Havalimanı transferi:** Araç tipi seçimi, gidiş-dönüş seçeneği, rezervasyon
- **Para birimi:** Fiyatların TL / USD / EUR olarak gösterilmesi
- **E-posta bildirimi:** Rezervasyon sonrası PNR'li onay e-postası (MailKit)
- **Üyelik/Giriş:** Hash'lenmiş şifreyle kayıt, çerez tabanlı oturum, oturuma duyarlı menü

### Yönetici Paneli (Admin)
- **Ayrı yönetici girişi:** Müşteri giriş ekranından bağımsız, yalnızca `Admin` rolüne açık (`/Account/AdminLogin`)
- **Dashboard:** Uçuş, rezervasyon, gelir ve no-show özetleri + hızlı erişim
- **Uçuş yönetimi:** Ekleme, listeleme, düzenleme, silme, detay (tam CRUD)
- **Rezervasyon yönetimi:** Listeleme, ödeme durumu takibi, rezervasyon iptali
- **Yolcular:** Tüm rezervasyonlardaki yolcuların birleşik listesi, arama ve check-in durumu
- **Destinasyonlar:** Uçuş verisinden üretilen varış noktaları ve yoğunlukları
- **Ek hizmet rezervasyonları:** Araç, tur ve transfer rezervasyonlarının sekmeli takibi
- **Online check-in:** PNR ile sorgulama, koltuk atama ve biniş kartı üretimi
- **Profil ve ayarlar:** Hesap bilgileri, çalışan şifre değiştirme, sistem bilgileri
- **Gerçek verili bildirimler:** Gecikmeli/iptal uçuşlar, son 24 saatteki rezervasyonlar

### Yapay Zeka ve Makine Öğrenmesi
- **AI Seyahat Asistanı:** Kullanıcının mesajından şehri tespit eder, gerçek hava durumu servisini (Open-Meteo) çağırır ve LLM (Google Gemini) ile öneri oluşturur — araç kullanan (tool calling) bir agent yapısı. Servis kotası dolduğunda hava durumu verisinden yedek öneri üretir.
- **No-Show Tahmini (ML.NET):** Geçmiş verilerle eğitilen FastTree regresyon modeli; bir uçuşta kaç yolcunun gelmeyeceğini tahmin eder
- **Uçuş Talep Tahmini (ML.NET):** Slot ve mevsime göre kaç bilet satılacağını tahmin eden ikinci regresyon modeli
- **Overbooking Önerisi ve Tahmin Paneli:** Slot bazlı no-show oranlarından risk seviyesi ve fazla satılabilir koltuk önerisi

## 🛠️ Kullanılan Teknolojiler

| Alan | Teknoloji |
|------|-----------|
| Backend | ASP.NET Core MVC (.NET 10) |
| Veritabanı | MongoDB (MongoDB.Driver) |
| Nesne Eşleme | AutoMapper |
| Makine Öğrenmesi | ML.NET (FastTree Regresyon) |
| Yapay Zeka | Google Gemini API |
| Hava Durumu | Open-Meteo (ücretsiz) |
| E-posta | MailKit + Mailpit (geliştirme ortamı) |
| Kimlik Doğrulama | Cookie Authentication + PasswordHasher, rol bazlı yetkilendirme |
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

4. **(E-posta için opsiyonel)** Rezervasyon onay e-postalarını görmek için Mailpit'i çalıştırın:
   ```bash
   docker run -d --name mailpit -p 8025:8025 -p 1025:1025 axllent/mailpit
   ```
   Gelen kutusu: `http://localhost:8025` — Mailpit kapalıyken uygulama sorunsuz çalışır, e-posta sessizce atlanır.

5. Uygulamayı çalıştırın:
   ```bash
   dotnet run
   ```

6. Tarayıcıdan açın: `http://localhost:5199`

### Demo Hesaplar
Uygulama ilk açılışta aşağıdaki hesapları otomatik oluşturur (giriş ekranlarında da gösterilir):

| Rol | E-posta | Şifre | Giriş adresi |
|-----|---------|-------|--------------|
| Yönetici | `admin@elasoft.com` | `Admin123!` | `/Account/AdminLogin` |
| Müşteri | `demo@elasoft.com` | `Demo123!` | `/Account/Login` |

### İlk Kullanım
- Araç, tur ve transfer örnek verileri ilk açılışta veritabanına otomatik yüklenir.
- Overbooking ve ML tahmin sayfalarındaki **"Örnek Veri Yükle"** butonuyla tahmin modelleri için demo verisi oluşturabilirsiniz.
- Ödeme ekranını denemek için örnek kart: `4242 4242 4242 4242`, son kullanma `12/28`, CVV `123`.

## 🧪 Testler

İş kurallarını doğrulayan **51 birim testi** bulunuyor (xUnit). Testler; ödeme ekranındaki kart doğrulamasını, check-in ek hizmet ücreti hesabını, para birimi dönüşümünü ve AI asistanının niyet tespitini kapsar. Veritabanı bağlantısı gerektirmezler.

```bash
dotnet test
```

## 📁 Proje Yapısı

```
FlightBooking.Tests/    # xUnit birim testleri (ödeme, ek hizmet, para birimi, agent)
FlightBooking/
├── Areas/Admin/        # Yönetici paneli (controller, view, component)
├── Controllers/        # Public controller'lar (Default, Flight, CarRental, Tour,
│                       #   Transfer, Agent, Account, Info, Preferences)
├── Entities/           # MongoDB varlıkları (Flight, Booking, Passenger, Car, Tour, ...)
├── Dtos/               # Veri transfer nesneleri
├── Services/           # İş katmanı servisleri (Flight, Booking, CheckIn, CarRental,
│                       #   Tour, Transfer, Email, ML, Auth)
├── AgentServices/      # AI agent (Gemini, WeatherTool, TravelAgent, niyet/şehir tespiti)
├── MachineLearningModels/  # ML.NET giriş/çıkış modelleri
├── Helpers/            # Para birimi biçimlendirme yardımcısı
├── Mapping/            # AutoMapper profilleri
├── Settings/           # Veritabanı ve API ayarları
└── wwwroot/geair/      # Arayüz şablonu ve statik dosyalar
```

## 📝 Notlar
- Bu proje eğitim amacıyla, adım adım geliştirilmiştir.
- MongoDB koleksiyonları (`Flights`, `Bookings`, `CheckIns`, `Cars`, `CarReservations`, `Tours`, `TourReservations`, `TransferVehicles`, `TransferReservations`, `Users`) uygulama çalışırken otomatik oluşur.
- Ödeme akışı bir simülasyondur; kart bilgileri saklanmaz, yalnızca biçim doğrulaması yapılır.
- Arayüzde dil seçici bulunur; tam İngilizce çeviri sonraki aşamada tamamlanacaktır.
