using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uduino;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class SensorTester : MonoBehaviour
{
    private FirebaseFirestore db;
    private UduinoManager UduManager;
    private UduinoDevice inputDevice = null;
    private UduinoDevice outputDevice = null;

    // SensorPackage ID
    [SerializeField] private string sensorPackageID;

    // Input
    [SerializeField] private int temperatureF;
    [SerializeField] private double temperatureC;
    [SerializeField] private int lightLevel;
    [SerializeField] private int waterLevel;
    [SerializeField] private int flameDetected;
    [SerializeField] private double humanDetected;
    [SerializeField] private int buttonPressed;
    // [SerializeField] private float distance = 0;

    // Output
    [Range(0, 255)] public int redIntensity;
    [Range(0, 255)] public int greenIntensity;
    [Range(0, 255)] public int blueIntensity;

    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        UduManager = UduinoManager.Instance;

        sensorPackageID = this.gameObject.name;

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
        if (UduManager.hasBoardConnected())
        {
            inputDevice = UduManager.GetBoard(sensorPackageID + "_in");
            outputDevice = UduManager.GetBoard(sensorPackageID + "_out");

            // Check if the board is connected
            if (inputDevice != null)
            {
                ProcessInputData();
            }

            if (outputDevice != null)
            {
                ProcessOutputData();
            }
        }
        else
        {
            Debug.Log("The boards have not been detected");
        }

    }

    // Process inputDevice data
    void ProcessInputData()
    {
        Debug.Log("[Input Board] " + inputDevice.name + " is connected");

        // Temperature Sensor : Pin A0
        UduManager.pinMode(inputDevice, AnalogPin.A0, PinMode.Input);

        // Light Sensor : Pin A1
        UduManager.pinMode(inputDevice, AnalogPin.A1, PinMode.Input);

        // Water Sensor : Pin A2
        UduManager.pinMode(inputDevice, AnalogPin.A2, PinMode.Input);

        // Flame Sensor : Pin A3
        UduManager.pinMode(inputDevice, AnalogPin.A3, PinMode.Input);

        // Human Detection Sensor : Pin 2
        UduManager.pinMode(inputDevice, 2, PinMode.Input_pullup);

        // Button : Pin 4
        UduManager.pinMode(inputDevice, 4, PinMode.Input_pullup);

        // Temperature Sensor
        temperatureF = UduManager.analogRead(inputDevice, AnalogPin.A0);
        temperatureC = System.Math.Round(temperatureF * 0.48828125, 1);

        // Light Sensor
        lightLevel = UduManager.analogRead(inputDevice, AnalogPin.A1);

        // Water Sensor
        waterLevel = UduManager.analogRead(inputDevice, AnalogPin.A2);

        // Flame Sensor
        flameDetected = UduManager.analogRead(inputDevice, AnalogPin.A3);

        // Human Detection Sensor
        humanDetected = (double)(UduManager.digitalRead(inputDevice, 2));

        // Button
        buttonPressed = UduManager.digitalRead(inputDevice, 4);

        // Result Log
        string resultLog = "Temperature: " + temperatureC + " || Light: " + lightLevel + " || Water: " + waterLevel + " || Flame: " + flameDetected + " || Human: " + humanDetected + " || Button: " + buttonPressed;

        //Debug.Log(resultLog);
    }

    // Process outputDevice data
    void ProcessOutputData()
    {
        // Debug.Log("Board2 is connected");

        // RGB LED : Pin 9, 10, 11
        UduManager.pinMode(outputDevice, 9, PinMode.Output);
        UduManager.pinMode(outputDevice, 10, PinMode.Output);
        UduManager.pinMode(outputDevice, 11, PinMode.Output);

        // RGB LED
        UduManager.analogWrite(outputDevice, 9, redIntensity);
        UduManager.analogWrite(outputDevice, 10, greenIntensity);
        UduManager.analogWrite(outputDevice, 11, blueIntensity);

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

        UduManager.sendCommand(outputDevice, displayMessage);
    }

    //void DataReceived(string data, UduinoDevice baord)
    //{
    //    bool ok = float.TryParse(data, out distance);
    //}
}