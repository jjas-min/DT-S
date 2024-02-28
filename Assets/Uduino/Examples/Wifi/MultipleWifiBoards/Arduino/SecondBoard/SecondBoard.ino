// Uduino settings
#include <Uduino_Wifi.h>
Uduino_WiFi uduino("secondBoard"); // Declare and name your object

void setup()
{
  Serial.begin(9600);
  uduino.addCommand("startLoop", StartLoop);

  uduino.setPort(4223); // You need to define a port different than 4222
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

    // Sending various elements;
    // String
    uduino.println("Printing a string.");

    // words and chars
    uduino.print("Printing a ");
    uduino.print("serie of ");
    uduino.print("words.");
    uduino.println();

    // char
    uduino.println('a');

    // int
    uduino.println(0);

    // float: the second parameter after the coma is the number of decimals
    uduino.println(0.123456789, 5);

    //Wait 2 second before sending the new message
    uduino.delay(2000); // Note, we use uduino.delay() to avoid bugs
  }
}
