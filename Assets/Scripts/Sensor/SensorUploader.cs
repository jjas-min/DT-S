using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Uduino;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;

public class SensorUploader : MonoBehaviour
{
    private FirebaseFirestore db;
    private float elapsedTime = 0f;

    UduinoManager UduManager;
    UduinoDevice inputDevice = null;
    UduinoDevice outputDevice = null;

    [SerializeField] private string sensorPackageID;

    // Input
    public int temperatureF;
    public double temperatureC;
    public int lightLevel;
    public int waterLevel;
    public int flameDetected;
    public double humanDetected;
    public int buttonPressed;

    public List<int> temperatureFs = new List<int>();
    public List<double> temperatureCs = new List<double>();
    public List<int> lightLevels = new List<int>();
    public List<int> waterLevels = new List<int>();
    public List<int> flameDetecteds = new List<int>();
    public List<double> humanDetecteds = new List<double>();
    public List<int> buttonPresseds = new List<int>();

    // public float distance = 0;

    // Output
    [Range(0, 255)] public int redIntensity;
    [Range(0, 255)] public int greenIntensity;
    [Range(0, 255)] public int blueIntensity;

    private string resultLog;

    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        UduManager = UduinoManager.Instance;

        sensorPackageID = this.gameObject.name;
        // UduinoManager.Instance.OnDataReceived += DataReceived;
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
            inputDevice = UduinoManager.Instance.GetBoard(sensorPackageID + "_in");
            outputDevice = UduinoManager.Instance.GetBoard(sensorPackageID + "_out");

            // Check if the board is connected
            if (inputDevice != null)
            {
                ProcessInputData();
                // //Debug.Log("Board1 is connected");

                // // Temperature Sensor : Pin A0
                // UduManager.pinMode(inputDevice, AnalogPin.A0, PinMode.Input);

                // // Light Sensor : Pin A1
                // UduManager.pinMode(inputDevice, AnalogPin.A1, PinMode.Input);

                // // Water Sensor : Pin A2
                // UduManager.pinMode(inputDevice, AnalogPin.A2, PinMode.Input);

                // // Flame Sensor : Pin A3
                // UduManager.pinMode(inputDevice, AnalogPin.A3, PinMode.Input);

                // // Human Detection Sensor : Pin 2
                // UduManager.pinMode(inputDevice, 2, PinMode.Input_pullup);

                // // Button : Pin 4
                // UduManager.pinMode(inputDevice, 4, PinMode.Input_pullup);

                // // Temperature Sensor
                // temperatureF = UduManager.analogRead(inputDevice, AnalogPin.A0);
                // temperatureC = System.Math.Round(temperatureF * 0.48828125, 1);

                // // Light Sensor
                // lightLevel = UduManager.analogRead(inputDevice, AnalogPin.A1);

                // // Water Sensor
                // waterLevel = UduManager.analogRead(inputDevice, AnalogPin.A2);

                // // Flame Sensor
                // flameDetected = UduManager.analogRead(inputDevice, AnalogPin.A3);

                // // Human Detection Sensor
                // humanDetected = (double)(UduManager.digitalRead(inputDevice, 2));

                // // Button
                // buttonPressed = UduManager.digitalRead(inputDevice, 4);

                // // Result Log
                // resultLog = "Temperature: " + temperatureC + " || Light: " + lightLevel + " || Water: " + waterLevel + " || Flame: " + flameDetected + " || Human: " + humanDetected + " || Button: " + buttonPressed;
                // dataAdd(temperatureC, lightLevel, waterLevel, flameDetected, humanDetected);
                //Debug.Log(resultLog);
            }
            if (outputDevice != null)
            {
                ProcessOutputData();
                // // Debug.Log("Board2 is connected");

                // // RGB LED : Pin 9, 10, 11
                // UduManager.pinMode(outputDevice, 9, PinMode.Output);
                // UduManager.pinMode(outputDevice, 10, PinMode.Output);
                // UduManager.pinMode(outputDevice, 11, PinMode.Output);

                // // RGB LED
                // UduManager.analogWrite(outputDevice, 9, redIntensity);
                // UduManager.analogWrite(outputDevice, 10, greenIntensity);
                // UduManager.analogWrite(outputDevice, 11, blueIntensity);

                // // LCD Display
                // // lightLevel을 3자리 숫자로 변환하여 displayValue에 저장
                // string displayMessage = "DisplayData";

                // // Display Light Level
                // displayMessage += " " + "L:";
                // if (lightLevel.ToString().Length < 3)
                // {
                //     displayMessage += lightLevel.ToString().PadLeft(3, '0');
                // }
                // else
                // {
                //     displayMessage += lightLevel;
                // }

                // // Display Temperature
                // displayMessage += " " + "T:";
                // displayMessage += temperatureC;

                // // Display Water Level
                // displayMessage += " " + "W:";
                // if (waterLevel.ToString().Length < 3)
                // {
                //     displayMessage += waterLevel.ToString().PadLeft(3, '0');
                // }
                // else
                // {
                //     displayMessage += waterLevel;
                // }

                // // Debug.Log("Display Value: " + displayMessage);

                // UduManager.sendCommand(outputDevice, displayMessage);
            }
        }
        else
        {
            Debug.Log("The boards have not been detected");
        }
        if (inputDevice != null && elapsedTime >= 10f)
        {
            UploadToDB();
            elapsedTime = 0f;
        }

        elapsedTime += Time.deltaTime;
    }

    //void DataReceived(string data, UduinoDevice baord)
    //{
    //    bool ok = float.TryParse(data, out distance);
    //}

    private void DataAdd(double temperatureC, int lightLevel, int waterLevel, int flameDetected, double humanDetected)
    {
        temperatureCs.Add(temperatureC);
        lightLevels.Add(lightLevel);
        waterLevels.Add(waterLevel);
        flameDetecteds.Add(flameDetected);
        humanDetecteds.Add(humanDetected);
        return;
    }

    private void UploadToDB()
    {
        Debug.Log("DB Uploading...");
        Timestamp tm = Timestamp.FromDateTime(DateTime.UtcNow);
        Dictionary<string, object> sensorDict = new Dictionary<string, object>
        {
            {"createdTime",     tm}, // DateTime ???? ???? ????
            {"temperature",     GetAverage(temperatureCs) },
            {"lightLevel",      GetAverage(lightLevels) },
            {"waterLevel",      GetAverage(waterLevels) },
            {"flameDetected",   GetAverage(flameDetecteds) },
            {"humanDetected",   GetAverage(humanDetecteds) },
        };
        if(Convert.ToDouble(sensorDict["flameDetected"]) > 0)
        {
            string[] parts = sensorPackageID.Split('_');
            Dictionary<string, object> alertDict = new Dictionary<string, object>
            {
                {"createdTime", tm},
                {"location", parts[0]},
                {"sensorPackageNum", parts[1]},
                {"flameDetected",   GetAverage(flameDetecteds) },
            };
            db.Collection("IssueAlerts").Document(tm.ToString()).SetAsync(alertDict);
        }
        db.Collection("SensorPackages").Document(sensorPackageID).Collection("SensorData").Document(tm.ToString()).SetAsync(sensorDict);
        ResetLists();
        return;
    }

    private double GetAverage(List<double> list)
    {
        double result = 0;
        foreach (var itm in list)
        {
            result += itm;
        }

        result /= list.Count;
        result = Math.Round(result * 100) / 100;
        return result;
    }

    private int GetAverage(List<int> list)
    {
        int result = 0;
        foreach (var itm in list)
        {
            result += itm;
        }

        result /= list.Count;
        return result;
    }

    private void ResetLists()
    {
        temperatureCs.Clear();
        waterLevels.Clear();
        lightLevels.Clear();
        flameDetecteds.Clear();
        humanDetecteds.Clear();
        return;
    }

    void ProcessInputData()
    {
        //Debug.Log("Board1 is connected");

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
        resultLog = "Temperature: " + temperatureC + " || Light: " + lightLevel + " || Water: " + waterLevel + " || Flame: " + flameDetected + " || Human: " + humanDetected + " || Button: " + buttonPressed;
        DataAdd(temperatureC, lightLevel, waterLevel, flameDetected, humanDetected);
    }

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

        // Debug.Log("Display Value: " + displayMessage);

        UduManager.sendCommand(outputDevice, displayMessage);
    }
}