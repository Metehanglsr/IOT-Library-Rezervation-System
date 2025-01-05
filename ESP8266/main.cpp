#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>

#define WIFI_SSID "your wifi ssid"
#define WIFI_PASSWORD "your wifi password"

#define TRIG_PIN1 D1
#define ECHO_PIN1 D2
#define TRIG_PIN2 D3
#define ECHO_PIN2 D4

String apiUrl = "your api url";

WiFiClient client;
HTTPClient http;

void setup() {
  Serial.begin(9600);
  pinMode(TRIG_PIN1, OUTPUT);
  pinMode(ECHO_PIN1, INPUT);
  pinMode(TRIG_PIN2, OUTPUT);
  pinMode(ECHO_PIN2, INPUT);

  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  Serial.print("Wi-Fi'ye bağlanılıyor");
  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println("\nWi-Fi'ye bağlanıldı!");
  Serial.print("IP adresi: ");
  Serial.println(WiFi.localIP());
}

float measureDistance(int trigPin, int echoPin) {
  digitalWrite(trigPin, LOW);
  delayMicroseconds(2);
  digitalWrite(trigPin, HIGH);
  delayMicroseconds(10);
  digitalWrite(trigPin, LOW);

  long duration = pulseIn(echoPin, HIGH);
  
  if (duration == 0) {
    Serial.println("Mesafe ölçüm hatası.");
    return -1;
  }

  float distance = (duration * 0.034) / 2;
  
  Serial.print("Ölçülen Mesafe: ");
  Serial.println(distance);

  return distance;
}

void sendHTTPRequest(float distance1, float distance2) {
  if (WiFi.status() == WL_CONNECTED) {
    http.begin(client, apiUrl);
    http.addHeader("Content-Type", "application/json");

    if (distance1 > 0 && distance2 > 0) {
      String payload = "[";
      payload += "{\"SensorId\":1, \"MeasurementValue\": " + String(distance1, 1) + "},";
      payload += "{\"SensorId\":2, \"MeasurementValue\": " + String(distance2, 2) + "}";
      payload += "]";

      int httpCode = http.POST(payload);

      if (httpCode > 0) {
        String response = http.getString();
        Serial.println("API Yanıtı: " + response);
      } else {
        Serial.println("API Hata: " + String(httpCode));
      }
    } else {
      Serial.println("Geçersiz mesafe ölçümü.");
    }

    http.end();
  } else {
    Serial.println("Wi-Fi bağlantısı yok.");
  }
}

void loop() {
  float distance1 = measureDistance(TRIG_PIN1, ECHO_PIN1);
  float distance2 = measureDistance(TRIG_PIN2, ECHO_PIN2);

  if (distance1 > 0 && distance2 > 0) {
    sendHTTPRequest(distance1, distance2);
  } else {
    Serial.println("Geçersiz mesafe değerleri, veriler gönderilmiyor.");
  }

  delay(5000);
}