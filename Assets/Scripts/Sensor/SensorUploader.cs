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
        Timestamp createdTime = Timestamp.FromDateTime(DateTime.UtcNow);

        Dictionary<string, object> sensorDict = new Dictionary<string, object>
        {
            {"createdTime",     createdTime}, // DateTime ???? ???? ????
            {"temperature",     GetAverage(temperatureCs) },
            {"lightLevel",      GetAverage(lightLevels) },
            {"waterLevel",      GetAverage(waterLevels) },
            {"flameDetected",   GetAverage(flameDetecteds) },
            {"humanDetected",   GetAverage(humanDetecteds) },
        };

        // Upload Alert if unusual activity is detected
        CheckForAlertAndUpload(sensorDict);

        // Upload sensor data to Firestore
        db.Collection("SensorPackages").Document(sensorPackageID).Collection("SensorData").Document(createdTime.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss")).SetAsync(sensorDict);
        ResetLists();

        return;
    }

    // Check if the sensor data is unusual and upload an alert to Firestore
    private void CheckForAlertAndUpload(Dictionary<string, object> sensorDict)
    {
        string[] parse = sensorPackageID.Split('_');

        bool uploadAlert = false;

        // Alert Dictionary
        Timestamp createdTime = (Timestamp)sensorDict["createdTime"];
        Dictionary<string, object> alertDict = new Dictionary<string, object>{
                {"createdTime", createdTime},
                {"location", parse[0]},
                {"sensorPackageNum", parse[1]},
            };

        string alertLog = "[Alert Upload] ";

        // Check for unusual activity
        if (Convert.ToDouble(sensorDict["temperature"]) > 30.0)
        {
            alertDict.Add("temperature", Convert.ToDouble(sensorDict["temperature"]));
            uploadAlert = true;

            alertLog += "Temperature: " + sensorDict["temperature"] + " ";
        }

        if (Convert.ToInt32(sensorDict["lightLevel"]) > 400)
        {
            alertDict.Add("lightLevel", Convert.ToInt32(sensorDict["lightLevel"]));
            uploadAlert = true;

            alertLog += "Light: " + sensorDict["lightLevel"] + " ";
        }

        if (Convert.ToInt32(sensorDict["waterLevel"]) > 400)
        {
            alertDict.Add("waterLevel", Convert.ToInt32(sensorDict["waterLevel"]));
            uploadAlert = true;

            alertLog += "Water: " + sensorDict["waterLevel"] + " ";
        }

        if (Convert.ToDouble(sensorDict["flameDetected"]) > 3.0)
        {
            alertDict.Add("flameDetected", Convert.ToDouble(sensorDict["flameDetected"]));
            uploadAlert = true;

            alertLog += "Flame: " + sensorDict["flameDetected"] + " ";
        }
        
        if (Convert.ToDouble(sensorDict["humanDetected"]) > 0.5)
        {
            alertDict.Add("humanDetected", Convert.ToDouble(sensorDict["humanDetected"]));
            uploadAlert = true;

            alertLog += "Human: " + sensorDict["humanDetected"] + " ";
        }

        // Upload the alert to Firestore
        if (uploadAlert)
        {
            Debug.Log(alertLog);
            db.Collection("IssueAlerts").Document(createdTime.ToDateTime().ToString("yyyy-MM-dd HH:mm:ss")).SetAsync(alertDict);
        }
    }

    private double GetAverage(List<double> list)
    {
        double result = 0;
        foreach (var item in list)
        {
            result += item;
        }

        result /= list.Count;
        result = Math.Round(result * 100) / 100;
        return result;
    }

    private int GetAverage(List<int> list)
    {
        int result = 0;
        foreach (var item in list)
        {
            result += item;
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
}