# Kütüphane Rezervasyon Sistemi

Bu proje, **ASP.NET Core MVC**, **Web API** ve **ESP8266** kullanılarak geliştirilmiş bir IoT tabanlı kütüphane rezervasyon sistemidir. Sistem, masaların doluluk durumunu izlemek için mesafe sensörlerini kullanır. Kullanıcılara masa rezervasyonu yapabilmeleri ve masaların durumlarını görüntüleyebilecekleri bir web arayüzü sağlar.

---

## Özellikler

### 1. Masa Takibi
- Her masa bir **mesafe sensörü** ile donatılmıştır.
- **ESP8266** mikrodenetleyicisi sensör verilerini okur ve sunucuya gönderir.
- Masalar:
  - Rezervasyonu olan kullanıcı oturduğunda **Dolu** olarak işaretlenir.
  - 15 dakika boyunca kullanıcı algılanmadığında **Boş** olarak işaretlenir.
  - Kullanıcı mola almadan kalkarsa **Boş** olarak işaretlenir, eğer mola alarak kalkarsa masanın durumu 15 dakika boyunca değişmez.
  - Eğer mola bittiyse ve kullanıcı hala gelmediyse masa **Boş** olarak işaretlenir.

### 2. Web Tabanlı Rezervasyon
- Kullanıcılar:
  - Gerçek zamanlı olarak masaları ve masaların durumlarını görebilir.
  - **MVC tabanlı arayüz** üzerinden masa rezervasyonu yapabilir.

### 3. API Entegrasyonu
- Tüm işlevler bir **RESTful Web API** ile desteklenmektedir.
- API, kullanıcı isteklerini, masa atamalarını ve sensör verisi işlemlerini yönetir.

---

## Kullanılan Teknolojiler

### Backend
- **ASP.NET Core MVC**: Kullanıcı arayüzü ve etkileşim.
- **Web API**: Arka plan mantığı ve veritabanı işlemleri.
- **Entity Framework Core**: PostgreSQL ile veri tabanı entegrasyonu.

### IoT
- **ESP8266**: Masa takibi için mikrodenetleyici.
- **Mesafe Sensörleri**: Masa doluluğunu algılamak için kullanılır.

### Veritabanı
- **PostgreSQL**: Masa, kullanıcı ve rezervasyon verilerini depolar.

---

## Kurulum ve Ayarlar

### 1. Projeyi Klonlayın
```bash
git clone https://github.com/Metehanglsr/IOT-Library-Rezervation-System.git
cd IOT-Library-Rezervation-System
```

### 2. Veritabanını Yapılandırın
- `appsettings.json` dosyasındaki bağlantı dizesini PostgreSQL kurulumunuza uygun şekilde güncelleyin.

### 3. Uygulamayı Çalıştırın
- Veritabanı güncellemelerini uygulayın:
  ```bash
  dotnet ef migrations add InitialCreate
  dotnet ef database update
  ```
- Uygulamayı başlatın:
  ```bash
  dotnet run
  ```

### 4. ESP8266'yı Yapılandırın
- Sağlanan firmware'i ESP8266'nıza yükleyin.
- Firmware kodunda Wi-Fi bilgilerini ve API bağlantı noktasını güncelleyin.

---

## Kullanım

### Kullanıcılar
1. Web arayüzünü açın.
2. Giriş yapın veya kayıt olun.
3. Masa durumu kontrol edin.
4. Masa rezervasyonu yapın.

---

## Proje Yapısı

```plaintext
├── Controllers          # API ve MVC controller'ları
├── Models               # Veri modelleri
├── Views                # MVC görünümleri
├── Data                 # Veritabanı bağlamı ve güncellemeler
├── wwwroot              # Statik dosyalar (CSS, JS, görseller)
├── ESP8266              # Mikrodenetleyici için firmware kodu
└── README.md            # Proje dokümentasyonu
```

---
