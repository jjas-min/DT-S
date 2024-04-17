using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorData : MonoBehaviour
{
    // Sensor data
    [SerializeField] private double temperature;
    [SerializeField] private int lightLevel;
    [SerializeField] private int waterLevel;
    [SerializeField] private int flameDetected;
    [SerializeField] private double humanDetected;

    [SerializeField] private string sensorPackageID;

    void Start()
    {
        sensorPackageID = this.gameObject.name;
    }

    // Getters
    public double GetTemperature()
    {
        return temperature;
    }

    public int GetLightLevel()
    {
        return lightLevel;
    }

    public int GetWaterLevel()
    {
        return waterLevel;
    }

    public int GetFlameDetected()
    {
        return flameDetected;
    }

    public double GetHumanDetected()
    {
        return humanDetected;
    }

    public string GetSensorPackageID()
    {
        return sensorPackageID;
    }

    // Set the sensor data
    public void SetSensorData(double temperature, int lightLevel, int waterLevel, int flameDetected, double humanDetected)
    {
        this.temperature = temperature;
        this.lightLevel = lightLevel;
        this.waterLevel = waterLevel;
        this.flameDetected = flameDetected;
        this.humanDetected = humanDetected;
    }
}
