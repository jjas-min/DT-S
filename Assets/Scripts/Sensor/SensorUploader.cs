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

    private UduinoManager UduManager;
    private UduinoDevice inputDevice = null;
    private UduinoDevice outputDevice = null;

    private string sensorPackageID;
    [SerializeField] private string inputDeviceName;
    [SerializeField] private string outputDeviceName;

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
    [SerializeField] private int humanDetected;
    [SerializeField] private int gasLevel;
    [SerializeField] private int buttonPressed;
    [SerializeField] private int pm25Level;
    [SerializeField] private int pm100Level;

    [SerializeField] private List<double> temperatureCs = new List<double>();
    [SerializeField] private List<int> lightLevels = new List<int>();
    [SerializeField] private List<int> waterLevels = new List<int>();
    [SerializeField] private List<int> flameDetecteds = new List<int>();
    [SerializeField] private List<int> humanDetecteds = new List<int>();
    [SerializeField] private List<int> gasLevels = new List<int>();
    [SerializeField] private List<int> buttonPresseds = new List<int>();
    [SerializeField] private List<int> pm25Levels = new List<int>();
    [SerializeField] private List<int> pm100Levels = new List<int>();

    // Output
    [Range(0, 255)] public int redIntensity;
    [Range(0, 255)] public int greenIntensity;
    [Range(0, 255)] public int blueIntensity;

    // Timer for communication with boards
    float timer = 0.0f;
    float interval = 0.5f; // 0.5��

    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        UduManager = UduinoManager.Instance;

        sensorPackageID = this.gameObject.name;
        inputDeviceName = sensorPackageID + "_wifi";
        outputDeviceName = sensorPackageID + "_out";

        UduManager.OnDataReceived += DataReceived;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; // ������ �� �ð� �߰�

        if (UduManager.hasBoardConnected())
        {
            inputDevice = UduManager.GetBoard(inputDeviceName);
            outputDevice = UduManager.GetBoard(outputDeviceName);

            // Ÿ�̸Ӱ� interval���� ū ��� �Լ� ����
            if (timer >= interval)
            {

                // Check if the input board is connected and process the input data
                if (inputDevice != null)
                {
                    // Temperature Sensor Read
                    ReadDataFrom((int)Pin.A0);
                    // Light Sensor Read
                    ReadDataFrom((int)Pin.A1);
                    // Water Sensor Read
                    ReadDataFrom((int)Pin.A2);
                    // Flame Sensor Read
                    ReadDataFrom((int)Pin.A3);
                    // Gas Sensor Read
                    ReadDataFrom((int)Pin.A4);

                    // PMS Sensor Read
                    ReadDataFrom((int)Pin.D7);

                    // Human Detection Sensor Read
                    ReadDataFrom((int)Pin.D4);
                }

                // Check if the output board is connected and process the output data
                if (outputDevice != null)
                {
                    ProcessOutputData();
                }

                timer = 0.0f; // Ÿ�̸� �ʱ�ȭ
            }
        }
        else
        {
            // Debug.Log("## NO BOARDS DETECTD ##");
        }

        if (inputDevice != null && elapsedTime >= 10f && temperatureCs.Count > 0)
        {
            UploadToDB();
            elapsedTime = 0f;
        }

        elapsedTime += Time.deltaTime;
    }

    #region Board Data Processing (Input & Output)

    void DataReceived(string receivedData, UduinoDevice device)
    {
        if (device.name == inputDeviceName)
        {
            ProcessInputData(receivedData);
        }
    }

    void ProcessInputData(string data)
    {
        //Debug.Log("[Input Board] " + inputDevice.name + " is connected");

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
                            case (int)Pin.A4:
                                gasLevel = sensorValue;
                                Debug.Log("Sensor value from " + inputDevice.name + ": G" + gasLevel);
                                break;
                            case (int)Pin.D4:
                                humanDetected = sensorValue;
                                Debug.Log("Sensor value from " + inputDevice.name + ": H" + humanDetected);
                                break;
                            case (int)Pin.D7:
                                pm25Level = sensorValue;
                                Debug.Log("Sensor value from " + inputDevice.name + ": PM2.5" + pm25Level);
                                break;
                            case (int)Pin.D8:
                                pm100Level = sensorValue;
                                Debug.Log("Sensor value from " + inputDevice.name + ": PM10.0" + pm100Level);
                                break;
                            default:
                                Debug.LogError("### Invalid Pin [" + pinValue + "] ###");
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

        AddDataToLists(temperatureC, lightLevel, waterLevel, flameDetected, humanDetected, gasLevel, pm25Level, pm100Level);
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
        string command;

        // If the pin is analog, use analog read
        if (pin >= (int)Pin.A0)
        {
            command = "r " + pin + " >";
        }
        // PMS Sensor
        else if (pin == (int)Pin.D7)
        {
            command = "rp >";
        }
        // If the pin is digital, use digital read
        else if (pin >= (int)Pin.D0)
        {
            command = "rd " + pin + " >";
        }
        else
        {
            Debug.LogError("### Invalid Pin Read ###");
            return;
        }

        //Debug.Log("Command: " + command);

        UduManager.sendCommand(inputDevice, command);
    }

    void WriteAnalogDataTo(int pin, int value)
    {
        string message = "a " + pin + " " + value;
        UduManager.sendCommand(outputDevice, message);
    }

    void WriteDisplay()
    {
        // LCD Display
        // lightLevel�� 3�ڸ� ���ڷ� ��ȯ�Ͽ� displayValue�� ����
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
    #endregion

    #region Database Upload (Sensor Data & Alert)
    private void AddDataToLists(double temperatureC, int lightLevel, int waterLevel, int flameDetected, int humanDetected, int gasLevel, int pm25Level, int pm100Level)
    {
        temperatureCs.Add(temperatureC);
        lightLevels.Add(lightLevel);
        waterLevels.Add(waterLevel);
        flameDetecteds.Add(flameDetected);
        humanDetecteds.Add(humanDetected);
        gasLevels.Add(gasLevel);
        pm25Levels.Add(pm25Level);
        pm100Levels.Add(pm100Level);
        return;
    }

    private void UploadToDB()
    {
        DateTime createdTime = DateTime.Now;

        Dictionary<string, object> sensorDict = new Dictionary<string, object>
        {
            {"createdTime",     createdTime}, // DateTime
            {"temperature",     GetMedian(temperatureCs) },
            {"lightLevel",      GetMedian(lightLevels) },
            {"waterLevel",      GetMedian(waterLevels) },
            {"flameDetected",   GetMedian(flameDetecteds) },
            {"humanDetected",   GetValueCount(humanDetecteds, 1) },
            {"gasLevel",        GetMedian(gasLevels) },
            {"pm25Level",       GetMedian(pm25Levels) },
            {"pm100Level",      GetMedian(pm100Levels) }
        };

        // Upload Alert if unusual activity is detected
        CheckForAlertAndUpload(sensorDict);

        // Upload sensor data to Firestore
        db.Collection("SensorPackages").Document(sensorPackageID).Collection("SensorData").Document(createdTime.ToString("yyyy-MM-dd HH:mm:ss")).SetAsync(sensorDict);

        Debug.Log("[Sensor Data Upload] " + sensorPackageID + " " + createdTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + sensorDict["temperature"] + " " + sensorDict["lightLevel"] + " " + sensorDict["waterLevel"] + " " + sensorDict["flameDetected"] + " " + sensorDict["humanDetected"] + " " + sensorDict["gasLevel"] + " " + sensorDict["pm25Level"] + " " + sensorDict["pm100Level"]);

        ResetLists();

        return;
    }

    // Check if the sensor data is unusual and upload an alert to Firestore
    private void CheckForAlertAndUpload(Dictionary<string, object> sensorDict)
    {
        string[] parse = sensorPackageID.Split('_');

        bool uploadAlert = false;

        // Alert Dictionary
        DateTime createdTime = (DateTime)sensorDict["createdTime"];
        Dictionary<string, object> alertDict = new Dictionary<string, object>{
                {"createdTime", createdTime},
                {"location", parse[0]},
                {"sensorPackageNum", parse[1]},
            };

        string alertLog = "[Alert Upload] ";

        // Check for unusual activity

        // Temperature is too high
        if (Convert.ToDouble(sensorDict["temperature"]) > 40.0)
        {
            alertDict.Add("temperature", Convert.ToDouble(sensorDict["temperature"]));
            uploadAlert = true;

            alertLog += "Temperature: " + sensorDict["temperature"] + " ";
        }

        // Light level is too high or too low
        if (Convert.ToInt32(sensorDict["lightLevel"]) > 400 || Convert.ToInt32(sensorDict["lightLevel"]) < 100)
        {
            alertDict.Add("lightLevel", Convert.ToInt32(sensorDict["lightLevel"]));
            uploadAlert = true;

            alertLog += "Light: " + sensorDict["lightLevel"] + " ";
        }

        // Water level is too high
        if (Convert.ToInt32(sensorDict["waterLevel"]) > 300)
        {
            alertDict.Add("waterLevel", Convert.ToInt32(sensorDict["waterLevel"]));
            uploadAlert = true;

            alertLog += "Water: " + sensorDict["waterLevel"] + " ";
        }

        // Flame detected
        if (Convert.ToDouble(sensorDict["flameDetected"]) > 13.0)
        {
            alertDict.Add("flameDetected", Convert.ToDouble(sensorDict["flameDetected"]));
            uploadAlert = true;

            alertLog += "Flame: " + sensorDict["flameDetected"] + " ";
        }

        //// Gas detected
        //if (Convert.ToInt32(sensorDict["gasLevel"]) > 300)
        //{
        //    alertDict.Add("gasLevel", Convert.ToInt32(sensorDict["gasLevel"]));
        //    uploadAlert = true;

        //    alertLog += "Gas: " + sensorDict["gasLevel"] + " ";
        //}

        //// Dust
        //if (Convert.ToInt32(sensorDict["pm25Level"]) > 40 || Convert.ToInt32(sensorDict["pm100Level"]) > 50)
        //{
        //    alertDict.Add("pm25Level", Convert.ToInt32(sensorDict["pm25Level"]));
        //    alertDict.Add("pm100Level", Convert.ToInt32(sensorDict["pm100Level"]));
        //    uploadAlert = true;

        //    alertLog += "Dust: " + sensorDict["pm25Level"] + " " + sensorDict["pm100Level"] + " ";
        //}

        // Human detected
        if (uploadAlert && Convert.ToInt32(sensorDict["humanDetected"]) > 30)
        {
            alertDict.Add("humanDetected", Convert.ToDouble(sensorDict["humanDetected"]));

            alertLog += "Human: " + sensorDict["humanDetected"] + " ";
        }

        // Upload the alert to Firestore
        if (uploadAlert)
        {
            Debug.Log(alertLog);
            db.Collection("IssueAlerts").Document(createdTime.ToString("yyyy-MM-dd HH:mm:ss")).SetAsync(alertDict);
        }
    }

    private double GetMedian(List<double> list)
    {
        if (list.Count == 0) return 0;

        list.Sort();
        double result = 0;
        int count = list.Count;

        if (count % 2 == 0)
        {
            result = (list[count / 2 - 1] + list[count / 2]) / 2;
        }
        else
        {
            result = list[count / 2];
        }

        return result;
    }

    private int GetMedian(List<int> list)
    {
        if (list.Count == 0) return 0;

        list.Sort();
        int result = 0;
        int count = list.Count;

        if (count % 2 == 0)
        {
            result = (list[count / 2 - 1] + list[count / 2]) / 2;
        }
        else
        {
            result = list[count / 2];
        }

        return result;
    }

    private double GetAverage(List<double> list)
    {
        if (list.Count == 0) return 0;

        double result = 0;
        foreach (var item in list)
        {
            result += item;
        }

        if (list.Count == 0) return 0; // Prevent division by zero

        result /= list.Count;
        result = Math.Round(result * 100) / 100;
        return result;
    }

    private int GetAverage(List<int> list)
    {
        if (list.Count == 0) return 0;

        int result = 0;
        foreach (var item in list)
        {
            result += item;
        }

        if (list.Count == 0) return 0; // Prevent division by zero

        result /= list.Count;
        return result;
    }

    private int GetValueCount(List<int> list, int value)
    {
        int count = 0;
        foreach (var item in list)
        {
            if (item == value)
            {
                count++;
            }
        }

        return count;
    }

    private void ResetLists()
    {
        temperatureCs.Clear();
        waterLevels.Clear();
        lightLevels.Clear();
        flameDetecteds.Clear();
        humanDetecteds.Clear();
        gasLevels.Clear();
        pm25Levels.Clear();
        pm100Levels.Clear();
        return;
    }
    #endregion
}