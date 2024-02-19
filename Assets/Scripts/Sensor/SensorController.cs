using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uduino;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class SensorController : MonoBehaviour
{
    private FirebaseFirestore db;
    UduinoManager UduManager;
    UduinoDevice firstDevice = null;
    UduinoDevice secondDevice = null;

    // Input
    public int temperatureF;
    public double temperatureC;
    public int lightLevel;
    public int waterLevel;
    public int flameDetected;
    public double humanDetected;
    public int buttonPressed;
    // public float distance = 0;

    // Output
    [Range(0, 255)] public int redIntensity;
    [Range(0, 255)] public int greenIntensity;
    [Range(0, 255)] public int blueIntensity;

    // UI Components
    [SerializeField] private Text temperatureText;
    [SerializeField] private Text lightLevelText;
    [SerializeField] private Text waterLevelText;
    [SerializeField] private Text flameDetectedText;
    [SerializeField] private Text humanDetectedText;
    [SerializeField] private Text buttonPressedText;

    private string resultLog;

    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        UduManager = UduinoManager.Instance;

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
        if (UduinoManager.Instance.hasBoardConnected())
        {
            firstDevice = UduinoManager.Instance.GetBoard("firstArduino");
            secondDevice = UduinoManager.Instance.GetBoard("secondArduino");

            // Check if the board is connected
            if (firstDevice != null)
            {
                // Debug.Log("Board1 is connected");

                // Temperature Sensor : Pin A0
                UduManager.pinMode(firstDevice, AnalogPin.A0, PinMode.Input);

                // Light Sensor : Pin A1
                UduManager.pinMode(firstDevice, AnalogPin.A1, PinMode.Input);

                // Water Sensor : Pin A2
                UduManager.pinMode(firstDevice, AnalogPin.A2, PinMode.Input);

                // Flame Sensor : Pin A3
                UduManager.pinMode(firstDevice, AnalogPin.A3, PinMode.Input);

                // Human Detection Sensor : Pin 2
                UduManager.pinMode(firstDevice, 2, PinMode.Input_pullup);

                // Button : Pin 4
                UduManager.pinMode(firstDevice, 4, PinMode.Input_pullup);

                // Temperature Sensor
                temperatureF = UduManager.analogRead(firstDevice, AnalogPin.A0);
                temperatureC = System.Math.Round(temperatureF * 0.48828125, 1);

                // Light Sensor
                lightLevel = UduManager.analogRead(firstDevice, AnalogPin.A1);

                // Water Sensor
                waterLevel = UduManager.analogRead(firstDevice, AnalogPin.A2);

                // Flame Sensor
                flameDetected = UduManager.analogRead(firstDevice, AnalogPin.A3);

                // Human Detection Sensor
                humanDetected = (double)(UduManager.digitalRead(firstDevice, 2));

                // Button
                buttonPressed = UduManager.digitalRead(firstDevice, 4);

                // Result Log
                resultLog = "Temperature: " + temperatureC + " || Light: " + lightLevel + " || Water: " + waterLevel + " || Flame: " + flameDetected + " || Human: " + humanDetected + " || Button: " + buttonPressed;

                //Debug.Log(resultLog);
            }
            if (secondDevice != null)
            {
                // Debug.Log("Board2 is connected");

                // RGB LED : Pin 9, 10, 11
                UduManager.pinMode(secondDevice, 9, PinMode.Output);
                UduManager.pinMode(secondDevice, 10, PinMode.Output);
                UduManager.pinMode(secondDevice, 11, PinMode.Output);

                // RGB LED
                UduManager.analogWrite(secondDevice, 9, redIntensity);
                UduManager.analogWrite(secondDevice, 10, greenIntensity);
                UduManager.analogWrite(secondDevice, 11, blueIntensity);

                // LCD Display
                // lightLevel을 3자리 숫자로 변환하여 displayValue에 저장
                string displayMessage = "DisplayData";

                // Display Light Level
                displayMessage += " " + "L:";
                if (lightLevel.ToString().Length < 3)
                {
                    displayMessage += lightLevel.ToString().PadLeft(3, '0');
                }
                else
                {
                    displayMessage += lightLevel;
                }

                // Display Temperature
                displayMessage += " " + "T:";
                displayMessage += temperatureC;
                
                // Display Water Level
                displayMessage += " " + "W:";
                if (waterLevel.ToString().Length < 3)
                {
                    displayMessage += waterLevel.ToString().PadLeft(3, '0');
                }
                else
                {
                    displayMessage += waterLevel;
                }
                
                Debug.Log("Display Value: " + displayMessage);

                UduManager.sendCommand(secondDevice, displayMessage);
            }
        }
        else
        {
            Debug.Log("The boards have not been detected");
        }

    }

    //void DataReceived(string data, UduinoDevice baord)
    //{
    //    bool ok = float.TryParse(data, out distance);
    //}
    
}