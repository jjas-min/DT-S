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
    [SerializeField] private int gasLevel;
    [SerializeField] private int pm25Level;
    [SerializeField] private int pm100Level;

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

    public int GetGasLevel()
    {
        return gasLevel;
    }

    public int GetPM25Level()
    {
        return pm25Level;
    }

    public int GetPM100Level()
    {
        return pm100Level;
    }

    public string GetSensorPackageID()
    {
        return sensorPackageID;
    }

    // Set the sensor data
    public void SetSensorData(double temperature, int lightLevel, int waterLevel, int flameDetected, double humanDetected, int gasLevel, int pm25Level, int pm100Level)
    {
        this.temperature = temperature;
        this.lightLevel = lightLevel;
        this.waterLevel = waterLevel;
        this.flameDetected = flameDetected;
        this.humanDetected = humanDetected;
        this.gasLevel = gasLevel;
        this.pm25Level = pm25Level;
        this.pm100Level = pm100Level;
    }
}
