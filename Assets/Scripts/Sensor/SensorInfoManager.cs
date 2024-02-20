using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SensorInfoManager : MonoBehaviour
{
    public GameObject sensorInfoPanel;

    public TMP_Text temperatureText; 
    public TMP_Text lightLevelText;
    public TMP_Text waterLevelText;
    public TMP_Text flameDetectedText;
    public TMP_Text humanDetectedText;

    private SensorData sensorData;

    void Start()
    {
        sensorInfoPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // If sensorInfoPanel is active, update the sensor information
        if (sensorInfoPanel.activeSelf)
        {
            UpdateSensorInformation();
        }
    }
    
    void OnMouseDown()
    {
        sensorData = GetComponent<SensorData>();

        if (sensorData != null && sensorInfoPanel != null)
        {
            UpdateSensorInformation();
            sensorInfoPanel.SetActive(true);
        }
    }

    public void OnCloseButtonClicked()
    {
        sensorInfoPanel.SetActive(false); // X ��ư Ŭ�� �� Information Panel ��Ȱ��ȭ
    }

    public void UpdateSensorInformation()
    {
        temperatureText.text = $"Temperature: {sensorData.GetTemperature()}";
        lightLevelText.text = $"Light Level: {sensorData.GetLightLevel()}";
        waterLevelText.text = $"Water Level: {sensorData.GetWaterLevel()}";
        flameDetectedText.text = $"Flame Detected: {sensorData.GetFlameDetected()}";
        humanDetectedText.text = $"Human Detected: {sensorData.GetHumanDetected()}";
    }
}
