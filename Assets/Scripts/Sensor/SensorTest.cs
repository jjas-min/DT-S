using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uduino;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class SensorTest : MonoBehaviour
{
    private FirebaseFirestore db;
    private UduinoManager UduManager;
    private UduinoDevice inputDevice = null;
    private UduinoDevice outputDevice = null;

    // SensorPackage ID
    [SerializeField] private string sensorPackageID;

    enum Pin
    {
        D0 = 0,
        D1 = 1,
        D2 = 2,
        D3 = 3,
        D4 = 4,
        D5 = 5,
        D6 = 6,
        D7 = 7,
        D8 = 8,
        D9 = 9,
        D10 = 10,
        D11 = 11,
        D12 = 12,
        D13 = 13,
        A0 = 14,
        A1 = 15,
        A2 = 16,
        A3 = 17,
        A4 = 18,
        A5 = 19,
    }

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

        //UduManager.OnBoardConnected += OnBoardConnected;

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

    // Different setups for each arduino board
    void OnBoardConnected(UduinoDevice connectedDevice)
    {
        //You can launch specific functions here
        if (connectedDevice.name == sensorPackageID + "_in")
        {
            inputDevice = connectedDevice;
        }
        else if (connectedDevice.name == sensorPackageID + "_out")
        {
            outputDevice = connectedDevice;
        }
    }

    void ProcessInputData()
    {
        //Debug.Log("[Input Board] " + inputDevice.name + " is connected");

        ReadDataFrom((int)Pin.A0);
        ReadDataFrom((int)Pin.A1);
        ReadDataFrom((int)Pin.A2);
        ReadDataFrom((int)Pin.A3);
        ReadDataFrom((int)Pin.D2);
        ReadDataFrom((int)Pin.D4);
    }

    void ProcessOutputData()
    {
        //Debug.Log("[Output Board] " + inputDevice.name + " is connected");

        WriteAnalogDataTo((int)Pin.D9, redIntensity);
        WriteAnalogDataTo((int)Pin.D10, greenIntensity);
        WriteAnalogDataTo((int)Pin.D11, blueIntensity);

        WriteDisplay();
    }

    void ReadDataFrom(int pin)
    {
        // Directly read the value from the sensor
        string data;
        // If the pin is analog, use analog read
        if (pin >= (int)Pin.A0)
        {
            data = UduManager.Read(inputDevice, "r " + pin);
        }
        else
        {
            data = UduManager.Read(inputDevice, "rd " + pin);
        }
       
        // Split the string and convert it to int
        if (!string.IsNullOrEmpty(data))
        {
            data = data.Trim();
            List<string> dataList = new List<string>(data.Split(' '));

            if (dataList.Count >= 2)
            {
                // Get the pin value from the received data
                if (int.TryParse(dataList[0], out int pinValue))
                {
                    // Get the sensor value from the received data
                    if (int.TryParse(dataList[1], out int sensorValue))
                    {
                        switch (pinValue)
                        {
                            case (int)Pin.A0:
                                temperatureF = sensorValue;
                                temperatureC = Math.Round(temperatureF * 0.48828125, 1);
                                Debug.Log("Sensor value from " + inputDevice.name + ": T" + temperatureC);
                                break;
                            case (int)Pin.A1:
                                lightLevel = sensorValue;
                                Debug.Log("Sensor value from " + inputDevice.name + ": L" + lightLevel);
                                break;
                            case (int)Pin.A2:
                                waterLevel = sensorValue;
                                Debug.Log("Sensor value from " + inputDevice.name + ": W" + waterLevel);
                                break;
                            case (int)Pin.A3:
                                flameDetected = sensorValue;
                                Debug.Log("Sensor value from " + inputDevice.name + ": F" + flameDetected);
                                break;
                            case (int)Pin.D2:
                                humanDetected = sensorValue;
                                Debug.Log("Sensor value from " + inputDevice.name + ": H" + humanDetected);
                                break;
                            case (int)Pin.D4:
                                buttonPressed = sensorValue;
                                Debug.Log("Sensor value from " + inputDevice.name + ": B" + buttonPressed);
                                break;
                            default:
                                Debug.LogError("### Invalid Pin ###");
                                break;
                        }
                    }
                    else
                    {
                        Debug.LogError("### Failed To Parse Value ###");
                    }
                }
                else
                {
                    Debug.LogError("### Failed To Parse Pin ###");
                }
            }
            else
            {
                Debug.LogError("### Insufficient data received ###");
            }
        }
    }

    void WriteAnalogDataTo(int pin, int value)
    {
        string message = "a " + pin + " " + value;
        UduManager.sendCommand(outputDevice, message);
    }

    void WriteDisplay()
    {
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

        //Debug.Log("Display Value: " + displayMessage);

        UduManager.sendCommand(outputDevice, displayMessage);
    }
}