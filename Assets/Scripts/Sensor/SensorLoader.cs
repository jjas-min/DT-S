using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

public class SensorLoader : MonoBehaviour
{
    // Firestore Collection Reference
    private FirebaseFirestore db;
    private CollectionReference sensorDataCollection;

    private UduinoDevice outputDevice = null;
    private UduinoManager UduManager;

    [SerializeField] private string sensorPackageID;
    private SensorData sensorData;

    // Start is called before the first frame update
    void Start()
    {
        // Set sensorPackageID to the name of this GameObject
        sensorPackageID = this.gameObject.name;

        // Find the SensorData component
        sensorData = GetComponent<SensorData>();

        // Get a reference to the sensors collection
        db = FirebaseFirestore.DefaultInstance;
        sensorDataCollection = db.Collection("SensorPackages").Document(sensorPackageID).Collection("SensorData");

        UduManager = UduinoManager.Instance;

        // Listen for changes in the sensor data
        ListenForSensorData();
    }

    // Update is called once per frame
    void Update()
    {
        outputDevice = UduinoManager.Instance.GetBoard(sensorPackageID + "_out");
    }

    // Listen for changes in the sensor data
    public void ListenForSensorData()
    {
        Query query = sensorDataCollection.OrderByDescending("createdTime").Limit(1);
        ListenerRegistration listener = query.Listen(snapshot =>
        {
            foreach (DocumentSnapshot documentSnapshot in snapshot.Documents)
            {
                Dictionary<string, object> receivedSensorData = documentSnapshot.ToDictionary();

                sensorData.SetSensorData(temperature: Convert.ToDouble(receivedSensorData["temperature"], System.Globalization.CultureInfo.InvariantCulture),
                                        lightLevel: Convert.ToInt32(receivedSensorData["lightLevel"]),
                                        waterLevel: Convert.ToInt32(receivedSensorData["waterLevel"]),
                                        flameDetected: Convert.ToInt32(receivedSensorData["flameDetected"]),
                                        humanDetected: Convert.ToDouble(receivedSensorData["humanDetected"], System.Globalization.CultureInfo.InvariantCulture),
                                        gasLevel: Convert.ToInt32(receivedSensorData["gasLevel"]),
                                        pm25Level: Convert.ToInt32(receivedSensorData["pm25Level"]),
                                        pm100Level: Convert.ToInt32(receivedSensorData["pm100Level"])
                                        );

                Debug.Log("[Sensor Data Load] " + sensorPackageID + " " + sensorData.GetTemperature() + " " + sensorData.GetLightLevel() + " " + sensorData.GetWaterLevel() + " " + sensorData.GetFlameDetected() + " " + sensorData.GetHumanDetected() + " " + sensorData.GetGasLevel() + " " + sensorData.GetPM25Level() + " " + sensorData.GetPM100Level());
            
                if (outputDevice != null)
                {
                    ProcessOutputData();
                }
            }
            
        });
    }

    void ProcessOutputData()
    {
        // Debug.Log("Board2 is connected");

        // [RGB LED]
        // RGB LED : Pin 9, 10, 11
        // UduManager.pinMode(outputDevice, 9, PinMode.Output);
        // UduManager.pinMode(outputDevice, 10, PinMode.Output);
        // UduManager.pinMode(outputDevice, 11, PinMode.Output);

        // // RGB LED
        // UduManager.analogWrite(outputDevice, 9, redIntensity);
        // UduManager.analogWrite(outputDevice, 10, greenIntensity);
        // UduManager.analogWrite(outputDevice, 11, blueIntensity);

        // [LCD Display]
        // lightLevel을 3자리 숫자로 변환하여 displayValue에 저장
        string displayMessage = "DisplayData";

        // Display Light Level
        displayMessage += " " + "L:";
        if (sensorData.GetLightLevel().ToString().Length < 3)
        {
            displayMessage += sensorData.GetLightLevel().ToString().PadLeft(3, '0');
        }
        else
        {
            displayMessage += sensorData.GetLightLevel();
        }

        // Display Temperature
        displayMessage += " " + "T:";
        displayMessage += sensorData.GetTemperature();

        // Display Water Level
        displayMessage += " " + "W:";
        if (sensorData.GetWaterLevel().ToString().Length < 3)
        {
            displayMessage += sensorData.GetWaterLevel().ToString().PadLeft(3, '0');
        }
        else
        {
            displayMessage += sensorData.GetWaterLevel();
        }

        // Debug.Log("Display Value: " + displayMessage);

        UduManager.sendCommand(outputDevice, displayMessage);
    }
}

