using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SensorInfoManager : MonoBehaviour
{
    [SerializeField] private GameObject sensorInfoPanel;

    private TMP_Text temperatureText; 
    private TMP_Text lightLevelText;
    private TMP_Text waterLevelText;
    private TMP_Text flameDetectedText;
    private TMP_Text humanDetectedText;

    private SensorData sensorData;

    void Start()
    {
        sensorInfoPanel.SetActive(false);

        // Find the TextMeshPro components among the children of the sensorInfoPanel
        temperatureText = sensorInfoPanel.transform.Find("Temperature").GetComponent<TMP_Text>();
        lightLevelText = sensorInfoPanel.transform.Find("LightLevel").GetComponent<TMP_Text>();
        waterLevelText = sensorInfoPanel.transform.Find("WaterLevel").GetComponent<TMP_Text>();
        flameDetectedText = sensorInfoPanel.transform.Find("FlameDetected").GetComponent<TMP_Text>();
        humanDetectedText = sensorInfoPanel.transform.Find("HumanDetected").GetComponent<TMP_Text>();
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
