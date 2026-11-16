
#include <Wire.h>
#include <math.h>

const int GREEN_BUTTON = 2;
const int RED_BUTTON = 3;
const int YELLOW_BUTTON = 4;
const int BLUE_BUTTON = 5;
const int ORANGE_BUTTON = 6;

const int leds[5] = {8, 9, 10, 11, 12};

const int STRUM_BUTTON = 7;

const int buttonPins[5] = {
  GREEN_BUTTON,
  RED_BUTTON,
  YELLOW_BUTTON,
  BLUE_BUTTON,
  ORANGE_BUTTON
};

const char* buttonNames[5] = {
  "GREEN",
  "RED",
  "YELLOW",
  "BLUE",
  "ORANGE"
};

bool previousButtonState[6];
unsigned long lastButtonTime[6];

const unsigned long debounceTime = 40;

int streak = 0;

int mpuAddress = 0;
bool mpuFound = false;

bool starPowerActive = false;

const float STAR_ON_ANGLE = 45.0;
const float STAR_OFF_ANGLE = 25.0;

void updateLEDs() {
  int ledsToLight = streak / 10;

  if (ledsToLight > 5) {
    ledsToLight = 5;
  }

  for (int i = 0; i < 5; i++) {
    digitalWrite(leds[i], i < ledsToLight ? HIGH : LOW);
  }
}

void resetStreak() {
  streak = 0;
  updateLEDs();
  Serial.println("STREAK,0");
}

bool detectMPU() {
  Wire.beginTransmission(0x68);

  if (Wire.endTransmission() == 0) {
    mpuAddress = 0x68;
    return true;
  }

  Wire.beginTransmission(0x69);

  if (Wire.endTransmission() == 0) {
    mpuAddress = 0x69;
    return true;
  }

  return false;
}

void initializeMPU() {
  if (!detectMPU()) {
    mpuFound = false;
    Serial.println("MPU,0");
    return;
  }

  Wire.beginTransmission(mpuAddress);
  Wire.write(0x6B);
  Wire.write(0);
  Wire.endTransmission();

  mpuFound = true;

  Serial.print("MPU,1,");
  Serial.println(mpuAddress, HEX);
}

void readGyroscope() {
  if (!mpuFound) {
    return;
  }

  Wire.beginTransmission(mpuAddress);
  Wire.write(0x3B);
  Wire.endTransmission(false);

  Wire.requestFrom(mpuAddress, 6, true);

  if (Wire.available() < 6) {
    return;
  }

  int16_t ax = Wire.read() << 8 | Wire.read();
  int16_t ay = Wire.read() << 8 | Wire.read();
  int16_t az = Wire.read() << 8 | Wire.read();

  float angle = atan2((float)ay, (float)az) * 57.2958;

  if (!starPowerActive && angle > STAR_ON_ANGLE) {
    starPowerActive = true;
    Serial.println("STAR,1");
  }

  if (starPowerActive && angle < STAR_OFF_ANGLE) {
    starPowerActive = false;
    Serial.println("STAR,0");
  }
}

void readButtons() {
  for (int i = 0; i < 5; i++) {
    bool currentState = digitalRead(buttonPins[i]);

    if (currentState != previousButtonState[i]) {
      if (millis() - lastButtonTime[i] > debounceTime) {
        lastButtonTime[i] = millis();
        previousButtonState[i] = currentState;

        if (currentState == LOW) {
          Serial.print("FRET,");
          Serial.print(buttonNames[i]);
          Serial.println(",1");
        } else {
          Serial.print("FRET,");
          Serial.print(buttonNames[i]);
          Serial.println(",0");
        }
      }
    }
  }

  bool strumState = digitalRead(STRUM_BUTTON);

  if (strumState != previousButtonState[5]) {
    if (millis() - lastButtonTime[5] > debounceTime) {
      lastButtonTime[5] = millis();
      previousButtonState[5] = strumState;

      if (strumState == LOW) {
        Serial.println("STRUM,1");
      } else {
        Serial.println("STRUM,0");
      }
    }
  }
}

void readUnityCommands() {
  if (!Serial.available()) {
    return;
  }

  String command = Serial.readStringUntil('\n');
  command.trim();

  if (command == "HIT") {
    streak++;

    if (streak > 50) {
      streak = 50;
    }

    updateLEDs();

    Serial.print("STREAK,");
    Serial.println(streak);
  }
  else if (command == "MISS") {
    resetStreak();
  }
  else if (command == "RESET") {
    resetStreak();
    starPowerActive = false;
  }
}

void setup() {
  Serial.begin(115200);
  Serial.setTimeout(10);

  Wire.begin();
  Wire.setClock(100000);

  for (int i = 0; i < 5; i++) {
    pinMode(buttonPins[i], INPUT_PULLUP);
    previousButtonState[i] = digitalRead(buttonPins[i]);
    lastButtonTime[i] = millis();
  }

  pinMode(STRUM_BUTTON, INPUT_PULLUP);
  previousButtonState[5] = digitalRead(STRUM_BUTTON);
  lastButtonTime[5] = millis();

  for (int i = 0; i < 5; i++) {
    pinMode(leds[i], OUTPUT);
    digitalWrite(leds[i], LOW);
  }

  initializeMPU();

  Serial.println("READY");
}

void loop() {
  readButtons();
  readGyroscope();
  readUnityCommands();
}
