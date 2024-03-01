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
    [SerializeField] private double humanDetected;
    [SerializeField] private int buttonPressed;

    [SerializeField] private List<double> temperatureCs = new List<double>();
    [SerializeField] private List<int> lightLevels = new List<int>();
    [SerializeField] private List<int> waterLevels = new List<int>();
    [SerializeField] private List<int> flameDetecteds = new List<int>();
    [SerializeField] private List<double> humanDetecteds = new List<double>();
    [SerializeField] private List<int> buttonPresseds = new List<int>();

    // Output
    [Range(0, 255)] public int redIntensity;
    [Range(0, 255)] public int greenIntensity;
    [Range(0, 255)] public int blueIntensity;

    // Timer for communication with boards
    float timer = 0.0f;
    float interval = 0.5f; // 0.5초

    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        UduManager = UduinoManager.Instance;

        sensorPackageID = this.gameObject.name;
        inputDeviceName = sensorPackageID + "_in";
        outputDeviceName = sensorPackageID + "_out";

        UduManager.OnDataReceived += DataReceived;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime; // 프레임 간 시간 추가

        if (UduManager.hasBoardConnected())
        {
            inputDevice = UduManager.GetBoard(inputDeviceName);
            outputDevice = UduManager.GetBoard(outputDeviceName);

            // 타이머가 interval보다 큰 경우 함수 실행
            if (timer >= interval)
            {

                // Check if the input board is connected and process the input data
                if (inputDevice != null)
                {
                    ReadDataFrom((int)Pin.A0);
                    ReadDataFrom((int)Pin.A1);
                    ReadDataFrom((int)Pin.A2);
                    ReadDataFrom((int)Pin.A3);
                    ReadDataFrom((int)Pin.D2);
                    ReadDataFrom((int)Pin.D4);
                }

                // Check if the output board is connected and process the output data
                if (outputDevice != null)
                {
                    ProcessOutputData();
                }

                timer = 0.0f; // 타이머 초기화
            }
        }
        else
        {
            Debug.Log("## NO BOARDS DETECTD ##");
        }

        if (inputDevice != null && elapsedTime >= 10f)
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

        // Result Log
        // string resultLog = "Temperature: " + temperatureC + " || Light: " + lightLevel + " || Water: " + waterLevel + " || Flame: " + flameDetected + " || Human: " + humanDetected + " || Button: " + buttonPressed;

        AddDataToLists(temperatureC, lightLevel, waterLevel, flameDetected, humanDetected);
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
        else if (pin >= (int)Pin.D0)
        {
            command = "rd " + pin + " >";
        }
        else
        {
            Debug.LogError("### Invalid Pin ###");
            return;
        }

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
    #endregion

    #region Database Upload (Sensor Data & Alert)
    private void AddDataToLists(double temperatureC, int lightLevel, int waterLevel, int flameDetected, double humanDetected)
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
        DateTime createdTime = DateTime.Now;

        Dictionary<string, object> sensorDict = new Dictionary<string, object>
        {
            {"createdTime",     createdTime}, // DateTime
            {"temperature",     GetAverage(temperatureCs) },
            {"lightLevel",      GetAverage(lightLevels) },
            {"waterLevel",      GetAverage(waterLevels) },
            {"flameDetected",   GetAverage(flameDetecteds) },
            {"humanDetected",   GetAverage(humanDetecteds) },
        };

        // Upload Alert if unusual activity is detected
        CheckForAlertAndUpload(sensorDict);

        // Upload sensor data to Firestore
        db.Collection("SensorPackages").Document(sensorPackageID).Collection("SensorData").Document(createdTime.ToString("yyyy-MM-dd HH:mm:ss")).SetAsync(sensorDict);

        Debug.Log("[Sensor Data Upload] " + sensorPackageID + " " + createdTime.ToString("yyyy-MM-dd HH:mm:ss") + " " + sensorDict["temperature"] + " " + sensorDict["lightLevel"] + " " + sensorDict["waterLevel"] + " " + sensorDict["flameDetected"] + " " + sensorDict["humanDetected"]);

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
            db.Collection("IssueAlerts").Document(createdTime.ToString("yyyy-MM-dd HH:mm:ss")).SetAsync(alertDict);
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
    #endregion
}