// Uduino settings
#include <Uduino_Wifi.h>
Uduino_WiFi uduino("firstBoard"); // Declare and name your object

void setup()
{
  Serial.begin(9600);
  uduino.addCommand("startLoop", StartLoop);
  uduino.connectWifi("SSID", "password");
}

bool sendLoop = false;

void StartLoop() {
  sendLoop = !sendLoop;
}


void loop()
{
  uduino.update();

  if (uduino.isConnected() ) {
    // String
    if (sendLoop) {
      uduino.println("This is a message from te first board.");
      uduino.delay(700);
    }
  }
}
