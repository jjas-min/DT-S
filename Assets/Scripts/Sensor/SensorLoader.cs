using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SensorLoader : MonoBehaviour
{
    // Firestore Collection Reference
    private FirebaseFirestore db;
    private CollectionReference sensorDataCollection;

    [SerializeField] private string sensorPackageID;

    // Sensor Data
    public int temperature;
    public int lightLevel;
    public int waterLevel;
    public int flameDetected;
    public int humanDetected;
    public int buttonPressed;

    // Start is called before the first frame update
    void Start()
    {
        // Set sensorPackageID to the name of this GameObject
        sensorPackageID = this.gameObject.name;

        // Get a reference to the sensors collection
        db = FirebaseFirestore.DefaultInstance;
        sensorDataCollection = db.Collection("SensorPackages").Document(sensorPackageID).Collection("SensorData");
        
        // Listen for changes in the sensor data
        ListenForSensorData();
    }

    // Update is called once per frame
    void Update()
    {

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

                temperature = Convert.ToInt32(sensorData["temperature"]);
                lightLevel = Convert.ToInt32(sensorData["lightLevel"]);
                waterLevel = Convert.ToInt32(sensorData["waterLevel"]);
                flameDetected = Convert.ToInt32(sensorData["flameDetected"]);
                humanDetected = Convert.ToInt32(sensorData["humanDetected"]);
                // buttonPressed = Convert.ToInt32(sensorData["buttonPressed"]);

                Debug.Log("New sensor data received: " + temperature + " " + lightLevel + " " + waterLevel + " " + flameDetected + " " + humanDetected);
            }
        });
    }
}

