#include <Wire.h>
#include <Adafruit_PN532.h>
#include <ESP32Servo.h>
#include <BLEDevice.h>
#include <BLEUtils.h>
#include <BLEServer.h>
#include <BLE2902.h>
// See the following for generating UUIDs:
// https://www.uuidgenerator.net/
#include <WiFi.h>
#include <WiFiClientSecure.h>
#include <ArduinoJson.h>

#define SDA_PIN 21
#define SCL_PIN 22
#define SERVO_MOTOR_ANGLE 50
#define REQUIRED_ACCESS_LEVEL 2
#define SERVICE_UUID        "931058ce-581b-4344-996e-aef3da80fc1d"
#define CHARACTERISTIC_UUID "7b411f1f-7e30-4fad-98b4-a746544d19cc"

#define WIFI_SSID "TP-Link_FA18"
#define WIFI_PASSWORD "Petronel200!"
IPAddress staticIP(192, 168, 0, 105); // Your desired static IP address
IPAddress gateway(192, 168, 0, 1);    // Your gateway IP address
IPAddress subnet(255, 255, 255, 0);   // Your subnet mask

Servo myServo;
//A12, GND A19 & 3v3 J19
int servoPin = 19;

Adafruit_PN532 nfc(SDA_PIN, SCL_PIN);

uint8_t SELECT_APDU[] = {
  0x00, /* CLA */
  0xA4, /* INS */
  0x04, /* P1  */
  0x00, /* P2  */
  0x05, /* Length of AID  */
  0xF2, 0x22, 0x22, 0x22, 0x22 /* AID  */
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

String firebaseUrl = "https://accesscontrolmobileapp-default-rtdb.europe-west1.firebasedatabase.app/";
IPAddress firebaseIp(34, 107, 226, 223);
WiFiClientSecure client;

void configureNfc() {
  nfc.begin();
  bool nfcConfigResult = nfc.SAMConfig();
  if (nfcConfigResult == false) {
    Serial.println("NFC module configuration failure...");
    //return;
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

  BLEDevice::init("AccessControlSystem");
  pServer = BLEDevice::createServer();
  pServer->setCallbacks(new MyServerCallbacks());
  pService = pServer->createService(SERVICE_UUID);
  pCharacteristic = pService->createCharacteristic(
                                         CHARACTERISTIC_UUID,
                                         BLECharacteristic::PROPERTY_READ |
                                         BLECharacteristic::PROPERTY_WRITE
                                       );

  //pCharacteristic->setValue("Hello World says Neil");
  pService->start();
  // BLEAdvertising *pAdvertising = pServer->getAdvertising();  // this still is working for backward compatibility
  pAdvertising = BLEDevice::getAdvertising();
  pAdvertising->addServiceUUID(SERVICE_UUID);
  pAdvertising->setScanResponse(true);
  pAdvertising->setMinPreferred(0x06);  // functions that help with iPhone connections issue
  pAdvertising->setMinPreferred(0x12);
  BLEDevice::startAdvertising();
  Serial.println("Characteristic defined! Now you can read it in your phone!");
}

void configureAp() {
  
  WiFi.config(staticIP, gateway, subnet);
  WiFi.begin(WIFI_SSID, WIFI_PASSWORD);
  Serial.println("\nConnecting");

  while(WiFi.status() != WL_CONNECTED){
      Serial.print(".");
      delay(100);
  }
  Serial.println("\nConnected to the WiFi network");
  Serial.print("Local ESP32 IP: ");
  Serial.println(WiFi.localIP());

  server.begin();
  client.setInsecure();
}

void setup(void) {
  Serial.begin(115200);
  myServo.attach(servoPin);
  configureNfc();
  configureBle();
  configureAp();
}

void loop(void) {

  if (ReadHostCardEmulation()) {
    requestReceived = true;
  } else if (ReadBluetoothMessages()) {
    requestReceived = true;
  } else if (ReadWiFiMessage()) {
    requestReceived = true;
  }

  if (requestReceived) {

    bool requestApproved = resolveRequest(receivedMessage);
    if (requestApproved) {
      myServo.write(SERVO_MOTOR_ANGLE);
      delay(10000);
      myServo.write(0);
    } else {
      Serial.println("Access denied");
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
      delay(10000);
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

bool resolveRequest(String userId) {
  
  String path;
  int hasRights;
  if (!client.connect(firebaseIp, 443)) {
    Serial.println("Connection failed!");
    hasRights = -1;
  }
  else {
    path = firebaseUrl + "users/" + userId + ".json";
    hasRights = verifyUserAccessRights(path);
    if (hasRights == -1) {
      path = firebaseUrl + "admins/" + userId + ".json";
      hasRights = verifyUserAccessRights(path);
    }
  }
  if (hasRights < 1) {
    return false;
  }
  else {
    return true;
  }
}

int verifyUserAccessRights(String path) {

  int result = -1;

  Serial.println("Connected to server!");

  client.println("GET " + path + " HTTP/1.0");
  client.println("Host: accesscontrolmobileapp-default-rtdb.europe-west1.firebasedatabase.app");
  client.println("Connection: close");
  client.println();

  String msg = "";
  while (client.connected()) {
    String line = client.readStringUntil('\n');
    if (line == "\r") {
      Serial.println("headers received");
      break;
    }
    msg += line;
  }
  String jsn = "";
  bool isParsing = false;
  while (client.available()) {
    char c = client.read();
    Serial.print(c);
    if (c == '{') {
      isParsing = true;
    }
    if (isParsing) {
      jsn += c;
    }
    if (c == '}') {
      isParsing = false;
    }
  }
  Serial.println(jsn);
  if (!jsn.isEmpty()) {
    DynamicJsonDocument doc(1024);
    DeserializationError error = deserializeJson(doc, jsn);
    if (error) {
      Serial.print("deserializeJson() failed: ");
      Serial.println(error.c_str());
      return false;
    }
    int accessLevel = doc["AccessLevel"];
    Serial.print("Access Level: ");
    Serial.println(accessLevel);
    if (accessLevel <= REQUIRED_ACCESS_LEVEL) {
      result = 1;
    }
    else {
      result = 0;
    }
  }
  client.stop();

  return result;
}
