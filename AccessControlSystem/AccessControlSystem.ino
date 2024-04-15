#include <Wire.h>
#include <Adafruit_PN532.h>
#include <ESP32Servo.h>

#define SDA_PIN 21
#define SCL_PIN 22


#define SERVO_MOTOR_ANGLE 50

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

void setup(void) {
  //configure baud rate
  Serial.begin(115200);
  Serial.println("ESP32 Access Control System!");

  //configure servo motor
  myServo.attach(servoPin);

  //configure nfc
  nfc.begin();
  uint32_t versiondata = nfc.getFirmwareVersion();
  if (!versiondata) {
    Serial.print("Didn't find PN53x board");
    while (1);
  }

  nfc.SAMConfig();
  Serial.println("Waiting for an NFC card...");
}

void loop(void) {
    ReadHostCardEmulation();
}

void ReadHostCardEmulation(){
  //Serial.println("Listening...");
  if (nfc.inListPassiveTarget()) {
    Serial.println("Something's there...");

    uint8_t response[255];
    uint8_t responseLength = sizeof(response);

    bool success = nfc.inDataExchange(SELECT_APDU, sizeof(SELECT_APDU), response, &responseLength);

    if ((success == true) && (responseLength != 2)) {
      //trigger servo motor
      myServo.write(SERVO_MOTOR_ANGLE);

      Serial.println("Sent");
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