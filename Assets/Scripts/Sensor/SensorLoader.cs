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

    UduinoDevice outputDevice = null;
    UduinoManager UduManager;

    [SerializeField] private string sensorPackageID;

    // Sensor Data
    [SerializeField] private double temperature;
    [SerializeField] private int lightLevel;
    [SerializeField] private int waterLevel;
    [SerializeField] private int flameDetected;
    [SerializeField] private double humanDetected;
    // public int buttonPressed;

    // Start is called before the first frame update
    void Start()
    {
        // Set sensorPackageID to the name of this GameObject
        sensorPackageID = this.gameObject.name;

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
                Dictionary<string, object> sensorData = documentSnapshot.ToDictionary();

                temperature = Convert.ToDouble(sensorData["temperature"]);
                lightLevel = Convert.ToInt32(sensorData["lightLevel"]);
                waterLevel = Convert.ToInt32(sensorData["waterLevel"]);
                flameDetected = Convert.ToInt32(sensorData["flameDetected"]);
                humanDetected = Convert.ToDouble(sensorData["humanDetected"]);
                // buttonPressed = Convert.ToInt32(sensorData["buttonPressed"]);

                Debug.Log("New sensor data received: " + temperature + " " + lightLevel + " " + waterLevel + " " + flameDetected + " " + humanDetected);
            
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
        displayMessage += temperature;

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

