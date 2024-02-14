using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uduino;

public class SensorController : MonoBehaviour
{
    UduinoManager UduManager;

    // Input
    public int temperatureF;
    public double temperatureC;
    public int light;
    public int waterLevel;
    public int flame;
    public int humanDetected;
    public int buttonPressed;
    public int soundLevel;
    // public float distance = 0;

    // Output
    [Range(0, 255)] public int redIntensity;
    [Range(0, 255)] public int greenIntensity;
    [Range(0, 255)] public int blueIntensity;
    private string resultLog;

    // Start is called before the first frame update
    void Start()
    {
        UduManager = UduinoManager.Instance;

        // Temperature Sensor : Pin A0
        UduManager.pinMode(AnalogPin.A0, PinMode.Input);
        
        // Light Sensor : Pin A1
        UduManager.pinMode(AnalogPin.A1, PinMode.Input);

        // Water Sensor : Pin A2
        UduManager.pinMode(AnalogPin.A2, PinMode.Input);

        // Flame Sensor : Pin A3
        UduManager.pinMode(AnalogPin.A3, PinMode.Input);

        // Human Detection Sensor : Pin 2
        UduManager.pinMode(2, PinMode.Input_pullup); 

        // Button : Pin 4
        UduManager.pinMode(4, PinMode.Input_pullup);

        // Sound Sensor : Pin A4
        UduManager.pinMode(AnalogPin.A4, PinMode.Input);

        // RGB LED : Pin 9, 10, 11
        UduManager.pinMode(9, PinMode.Output);
        UduManager.pinMode(10, PinMode.Output);
        UduManager.pinMode(11, PinMode.Output);

        // UduinoManager.Instance.OnDataReceived += DataReceived;

        // // Temperature + Humidity Sensor : Serial
        // UduManager.OnDataReceived += DataReceived;
    }

    // void DataReceived(string value, UduinoDevice board)
    // {
    //     Debug.Log("Value: " + value + " Board: " + board);
    // }

    // Update is called once per frame
    void Update()
    {
        // Temperature Sensor
        temperatureF = UduManager.analogRead(AnalogPin.A0);
        temperatureC = temperatureF * 0.48828125;

        // Light Sensor
        light = UduManager.analogRead(AnalogPin.A1);

        // Water Sensor
        waterLevel = UduManager.analogRead(AnalogPin.A2);

        // Flame Sensor
        flame = UduManager.analogRead(AnalogPin.A3);

        // Human Detection Sensor
        humanDetected = UduManager.digitalRead(2);

        // Button
        buttonPressed = UduManager.digitalRead(4);

        // Sound Sensor
        soundLevel = UduManager.analogRead(AnalogPin.A4);

        // RGB LED
        UduManager.analogWrite(9, redIntensity);
        UduManager.analogWrite(10, greenIntensity);
        UduManager.analogWrite(11, blueIntensity);

        // Result Log
        resultLog = "Temperature: " + temperatureC + " || Light: " + light + " || Water: " + waterLevel + " || Flame: " + flame + " || Human: " + humanDetected + " || Button: " + buttonPressed + " || Sound: " + soundLevel;
        Debug.Log(resultLog);
    }

    // void DataReceived(string data, UduinoDevice baord)
    // {
    //     bool ok = float.TryParse(data, out distance);
    // }
}


