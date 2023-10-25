#include "RotaryEncoder.h"
RotaryEncoder encoder(A2, A3);  // (DT, CLK)

static int pos = 0;
int newPos;

void setup() {
  Serial.begin(9600); // ouvre le port série
  Serial.println(pos);
}

void loop() {
   encoder.tick();
   newPos = encoder.getPosition();

   if (pos != newPos) {
      Serial.flush();
      Serial.println(newPos);
      pos = newPos; 
   }
}