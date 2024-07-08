#include <Wire.h>
#include <Adafruit_PN532.h>
#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>
#include <BLE2902.h>
#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <ArduinoJson.h>
#include <Firebase_ESP_Client.h>
#include <WiFiUdp.h>
#include <NTPClient.h>
#include <ESPmDNS.h>
#include <U8g2lib.h>

#include "addons/TokenHelper.h"
#include "addons/RTDBHelper.h"

#define SDA_PIN 21
#define SCL_PIN 22
#define CTRL_PIN 4
#define REQUIRED_ACCESS_LEVEL 2
#define GATE_ID "IE303"
#define SERVICE_UUID        "931058ce-581b-4344-996e-aef3da80fc1d"
#define CHARACTERISTIC_UUID "7b411f1f-7e30-4fad-98b4-a746544d19cc"

#define WIFI_SSID "MyNetwork"
#define WIFI_PASSWORD "Passw0rd!"
#define API_KEY "AIzaSyAHl-cJy8zniA0u7yQSu8hZ7avsToaAgEA"
#define DATABASE_URL "https://accesscontrolmobileapp-default-rtdb.europe-west1.firebasedatabase.app/" 

U8G2_SSD1306_128X64_NONAME_1_4W_SW_SPI u8g2(U8G2_R0, /* clock=*/ 18, /* data=*/ 23, /* cs=*/ 5, /* dc=*/ 17, /* reset=*/ 16);
Adafruit_PN532 nfc(SDA_PIN, SCL_PIN);

uint8_t SELECT_APDU[] = {
  0x00, /* CLA */
  0xA4, /* INS */
  0x04, /* P1  */
  0x00, /* P2  */
  0x05, /* Length of AID  */
  0xF5, 0x22, 0x22, 0x22, 0x22 /* AID  */
};

BLEServer* pServer = NULL;
BLEService* pService = NULL;
BLECharacteristic* pCharacteristic;
BLEAdvertising* pAdvertising;
bool deviceConnected = false;
bool oldDeviceConnected = false;

WiFiServer server(80);
String receivedMessage = "";
bool requestReceived = false;

WiFiClientSecure client;

FirebaseData fbdo;
FirebaseAuth auth;
FirebaseConfig config;
bool signupOK = false;

WiFiUDP ntpUDP;
NTPClient timeClient(ntpUDP, "pool.ntp.org", 3600, 60000); // NTP server, time offset in seconds, update interval in milliseconds
int accessLevel;
bool isAdmin;
bool isApproved;
String accessMethod;

void configureDisplay() {

  pinMode(5, OUTPUT);
  pinMode(17, OUTPUT);
  digitalWrite(5, 0);
  digitalWrite(17, 0);	

  u8g2.begin();

  ClearDisplay();
}

void configureNfc() {
  nfc.begin();
  bool nfcConfigResult = nfc.SAMConfig();
  if (nfcConfigResult == false) {
    Serial.println("NFC module configuration failure...");
  }
  Serial.println("Waiting for an NFC card...");
}

class MyServerCallbacks: public BLEServerCallbacks {
  void onConnect(BLEServer* pServer) {
    deviceConnected = true;
  };
  void onDisconnect(BLEServer* pServer) {
    deviceConnected = false;
  }
};

void configureBle() {
  Serial.println("Starting BLE...");

  BLEDevice::init("RoomIE303");
  pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());
  pService = pServer->createService(SERVICE_UUID);
  pCharacteristic = pService->createCharacteristic(
                                         CHARACTERISTIC_UUID,
                                         BLECharacteristic::PROPERTY_WRITE
                                       );
  pService->start();
  pServer->getAdvertising()->start();
  Serial.println("Waiting client connection...");
}

void configureAp() {
  
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  Serial.println("\nConnecting");

  while(WiFi.status() != WL_CONNECTED){
      Serial.print(".");
      delay(100);
  }
  Serial.println("\nConnected to the WiFi network");
  Serial.print("Local ESP32 IP: ");
  Serial.println(WiFi.localIP());

  if (!MDNS.begin(GATE_ID)) {
    Serial.println("Error setting up MDNS responder!");
  }
  Serial.println("mDNS responder started");

  server.begin();

  config.api_key = API_KEY;
  config.database_url = DATABASE_URL;

  if (Firebase.signUp(&config, &auth, "", "")) {
    signupOK = true;
  }
  else {
    Serial.printf("%s\n", config.signer.signupError.message.c_str());
  }

  config.token_status_callback = tokenStatusCallback;

  Firebase.begin(&config, &auth);
  Firebase.reconnectWiFi(true);

}

void setup(void) {
  Serial.begin(115200);
  configureDisplay();
  pinMode(CTRL_PIN, OUTPUT);
  configureNfc();
  configureBle();
  configureAp();
  timeClient.begin();
}

void loop(void) {

  //MDNS.update();
  if (ReadHostCardEmulation()) {
    requestReceived = true;
    accessMethod = "NFC";
  } else if (ReadBluetoothMessages()) {
    requestReceived = true;
    accessMethod = "Bluetooth";
  } else if (ReadWiFiMessage()) {
    requestReceived = true;
    accessMethod = "Wi-Fi";
  }

  if (requestReceived) {

    int requestStatus = VerifyUser(receivedMessage);
    if (requestStatus == 1) {
      isApproved = true;
      Serial.println("Access approved");
      digitalWrite(CTRL_PIN, HIGH);
      DisplayRequestStatus(isApproved);
      delay(3000);
      ClearDisplay();
      delay(7000);
      digitalWrite(CTRL_PIN, LOW);
      SaveLogsInsideDatabase(receivedMessage);
    } else if (requestStatus == 0) {
      isApproved = false;
      Serial.println("Access denied");
      DisplayRequestStatus(isApproved);
      delay(3000);
      ClearDisplay();
      SaveLogsInsideDatabase(receivedMessage);
    }
    else {
      Serial.println("User doesn't exist");
    }
    requestReceived = false;
    receivedMessage.clear();
  }
}

bool ReadHostCardEmulation() {
  bool nfcMessageReceived = false;
  if (nfc.inListPassiveTarget()) {
    Serial.println("Something's there...");

    uint8_t response[255];
    uint8_t responseLength = sizeof(response);

    bool success = nfc.inDataExchange(SELECT_APDU, sizeof(SELECT_APDU), response, &responseLength);

    if ((success == true) && (responseLength != 2)) {
      Serial.println(F("Sent"));
      nfcMessageReceived = true;
      nfc.PrintHexChar(response, responseLength);
      for (size_t i = 0; i < responseLength - 2; i++) {
        receivedMessage.concat((char)response[i]);
      }
      Serial.println(receivedMessage);
      delay(1000);
    } else {
      Serial.println("Not sent");
    }
  }
  delay(1000);
  return nfcMessageReceived;
}

bool ReadBluetoothMessages() {
  bool bluetoothMessageReceived = false;
  if (deviceConnected) {
    std::string value = pCharacteristic->getValue();
    if (!value.empty()) {
      Serial.print("Received message from phone: ");
      Serial.println(String(value.c_str()));
      pCharacteristic->setValue("");
      bluetoothMessageReceived = true;
      receivedMessage = String(value.c_str());
    }
    delay(1000);
  }
  if (!deviceConnected && oldDeviceConnected) {
      delay(500);
      pServer->startAdvertising();
      Serial.println("start advertising");
      oldDeviceConnected = deviceConnected;
  }
  if (deviceConnected && !oldDeviceConnected) {
      oldDeviceConnected = deviceConnected;
  }
  return bluetoothMessageReceived;
}

bool ReadWiFiMessage() {
  bool wifiMessageReceived = false;
  WiFiClient client = server.available();
  if (client) {
    Serial.println("New Client.");
    String postMessage = "";
    String currentLine = "";
    bool firstLine = true;
    bool isPostRequest = false;
    bool readPostMessage = false;
    while (client.connected()) {
      if (client.available()) {
        char c = client.read();
        if (c == '\n') {
          Serial.print('\\');
          Serial.print("n");
        }
        if (c == '\r') {
          Serial.print('\\');
          Serial.print("r");
        }
        if ((c != '\n') && (c != '\r')) {
          Serial.write(c);
        }

        if (postMessage.length() == 28) {
          break;
        }
        if (c == '\n') {
          if (currentLine.length() == 0) {
            if (isPostRequest == false) {
              break;
            }
            else {
              readPostMessage = true;
            }
          }
          else {
            if (firstLine) {
              if (currentLine.startsWith("POST")) {
                isPostRequest = true;
              }
              firstLine = false;
            }
            currentLine = "";
          }
        }
        else if (c != '\r') {
          if (readPostMessage) {
            postMessage += c;
          }
          currentLine += c;
        }
      }
    }
    if (postMessage.length() != 0){
      wifiMessageReceived = true;
      receivedMessage = postMessage;
    }
    client.stop();
    Serial.println("Client Disconnected.");
  }

  if (wifiMessageReceived) {
    Serial.println(receivedMessage);
  }
  return wifiMessageReceived;
}

int VerifyUser(String userId) {
  
  int okResult = -1;
  bool userExists = false;
  String nodePath = "/admins/" + userId;
  if (Firebase.RTDB.getJSON(&fbdo, nodePath.c_str())) {
    userExists = true;
  } else {
      nodePath = "/users/" + userId;
      if (Firebase.RTDB.getJSON(&fbdo, nodePath.c_str())) {
        userExists = true;
      }
      else {
        userExists = false;
        Serial.println(fbdo.errorReason());
      }
  }
  if (userExists) {
    if (fbdo.dataType() == "json") {
      FirebaseJson& json = fbdo.jsonObject();
      String jsonString;
      json.toString(jsonString, true);
      Serial.println("JSON data:");
      Serial.println(jsonString);
      GetUserData(jsonString);
      if (accessLevel <= REQUIRED_ACCESS_LEVEL) {
        okResult = 1;
      }
      else {
        okResult = 0;
      }
    }
  }
  return okResult;
}

void GetUserData(String json) {
  if (!json.isEmpty()) {
    DynamicJsonDocument doc(1024);
    DeserializationError error = deserializeJson(doc, json);
    if (error) {
      Serial.print("deserializeJson() failed: ");
      Serial.println(error.c_str());
    }
    accessLevel = doc["AccessLevel"];
    isAdmin = doc["IsAdmin"];
  }
}

void SaveLogsInsideDatabase(String userId) {

  String rawDate;
  String formattedDate;
  GetDateAndTimeString(rawDate, formattedDate);
  String nodePath = "/z_logs/" + rawDate + userId + "/UserId";
  Firebase.RTDB.setString(&fbdo, nodePath.c_str(), userId.c_str());
  nodePath = "/z_logs/" + rawDate + userId + "/IsAdmin";
  Firebase.RTDB.setString(&fbdo, nodePath.c_str(), isAdmin);
  nodePath = "/z_logs/" + rawDate + userId + "/AccessMethod";
  Firebase.RTDB.setString(&fbdo, nodePath.c_str(), accessMethod.c_str());
  nodePath = "/z_logs/" + rawDate + userId + "/GateId";
  Firebase.RTDB.setString(&fbdo, nodePath.c_str(), GATE_ID);
  nodePath = "/z_logs/" + rawDate + userId + "/IsApproved";
  Firebase.RTDB.setString(&fbdo, nodePath.c_str(), isApproved);
  nodePath = "/z_logs/" + rawDate + userId + "/DateAndTime";
  Firebase.RTDB.setString(&fbdo, nodePath.c_str(), formattedDate.c_str());
}

void GetDateAndTimeString(String &rawDate, String &formattedDate) {
  String dateAndTime;
  timeClient.update();
  unsigned long epochTime = timeClient.getEpochTime();
  struct tm *ptm = gmtime ((time_t *)&epochTime);
  int year = (ptm->tm_year + 1900) % 100;
  int month = ptm->tm_mon + 1;
  int day = ptm->tm_mday;
  int hour = ptm->tm_hour;
  int minute = ptm->tm_min;
  int second = ptm->tm_sec;
  char buffer[13];
  char formattedBuffer[20];
  snprintf(buffer, sizeof(buffer), "%02d%02d%02d%02d%02d%02d", year, month, day, hour, minute, second);
  snprintf(formattedBuffer, sizeof(formattedBuffer), "%02d:%02d:%02d %02d/%02d/%02d", hour, minute, second, day, month, year % 100);
  rawDate = (String)buffer;
  formattedDate =(String)formattedBuffer;
}

void ClearDisplay() {
  u8g2.firstPage();
  do {
    u8g2.drawFrame(2, 0, 126, 64);
  } while (u8g2.nextPage());
}

void DisplayRequestStatus(bool status) {
  if (status)
  {
    u8g2.firstPage();
    do {
      u8g2.setFont(u8g2_font_ncenB10_tr);
      u8g2.drawStr(2, 35, "Access approved");
      u8g2.drawFrame(2, 0, 126, 64);
    } while (u8g2.nextPage());
  }
  else {
    u8g2.firstPage();
    do {
      u8g2.setFont(u8g2_font_ncenB10_tr);
      u8g2.drawStr(12, 35, "Access denied");
      u8g2.drawFrame(2, 0, 126, 64);
    } while (u8g2.nextPage());
  }
}