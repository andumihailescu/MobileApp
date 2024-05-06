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

#define SDA_PIN 21
#define SCL_PIN 22
#define SERVO_MOTOR_ANGLE 50
#define SERVICE_UUID        "931058ce-581b-4344-996e-aef3da80fc1d"
#define CHARACTERISTIC_UUID "7b411f1f-7e30-4fad-98b4-a746544d19cc"

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
  0xF5, 0x22, 0x22, 0x22, 0x22 /* AID  */
};

BLEServer* pServer = NULL;
BLEService* pService = NULL;
BLECharacteristic* pCharacteristic;
BLEAdvertising* pAdvertising;
bool deviceConnected = false;
bool oldDeviceConnected = false;

const char* ssid = "AccessControlSystem";
const char* password = "Password";
WiFiServer server(80);

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
  if (!WiFi.softAP(ssid, password)) {
    log_e("Soft AP creation failed.");
    return;
  }
  IPAddress myIP = WiFi.softAPIP();
  Serial.print("AP IP address: ");
  Serial.println(myIP);
  server.begin();
  Serial.println("Server started");
}

void setup(void) {
  Serial.begin(115200);
  myServo.attach(servoPin);
  configureNfc();
  configureBle();
  configureAp();
}

void loop(void) {
    ReadHostCardEmulation();
    ReadBluetoothMessages();
    ReadWiFiMessage();
}

void ReadHostCardEmulation() {
  
  if (nfc.inListPassiveTarget()) {
    Serial.println("Something's there...");

    uint8_t response[255];
    uint8_t responseLength = sizeof(response);

    bool success = nfc.inDataExchange(SELECT_APDU, sizeof(SELECT_APDU), response, &responseLength);

    if ((success == true) && (responseLength != 2)) {
      
      myServo.write(SERVO_MOTOR_ANGLE);

      Serial.println(F("Sent"));
      nfc.PrintHexChar(response, responseLength);
      for (size_t i = 0; i < responseLength; i++) {
          Serial.print(response[i], HEX);
          Serial.print(' ');
      }
      Serial.println();

      delay(10000);
      myServo.write(0);
    } else {
      Serial.println("Not sent");
    }
  }
  delay(1000);
}

void ReadBluetoothMessages() {

  if (deviceConnected) {
    std::string value = pCharacteristic->getValue();
    if (!value.empty()) {
      Serial.print("Received message from phone: ");
      Serial.println(String(value.c_str()));
      pCharacteristic->setValue("");
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
}

void ReadWiFiMessage() {
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
    client.stop();
    Serial.println("Client Disconnected.");
  }
}


