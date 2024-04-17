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

    private TMP_Text sensorPackageIDText;

    [SerializeField] private SensorInfoPanelController sensorInfoPanelController;

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

        sensorPackageIDText = sensorInfoPanel.transform.Find("SensorPackageID").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        sensorData = GetComponent<SensorData>();

        // If sensorInfoPanel is active, update the sensor information
        if (sensorInfoPanel.activeSelf && sensorData.GetSensorPackageID() == sensorInfoPanelController.GetActiveSensorPackageID())
        {
            UpdateSensorInformation();
        }
    }
    
    void OnMouseDown()
    {
        sensorData = GetComponent<SensorData>();

        sensorInfoPanelController.SetActiveSensorPackageID(sensorData.GetSensorPackageID());

        if (sensorData != null && sensorInfoPanel != null && !sensorInfoPanel.activeSelf && sensorData.GetSensorPackageID() == sensorInfoPanelController.GetActiveSensorPackageID())
        {
            UpdateSensorInformation();
            sensorInfoPanel.SetActive(true);
        }
    }

    public void UpdateSensorInformation()
    {
        sensorPackageIDText.text = sensorData.GetSensorPackageID();
        temperatureText.text = $"{sensorData.GetTemperature()}";
        lightLevelText.text = $"{sensorData.GetLightLevel()}";
        waterLevelText.text = $"{sensorData.GetWaterLevel()}";
        flameDetectedText.text = $"{sensorData.GetFlameDetected()}";
        humanDetectedText.text = sensorData.GetHumanDetected() > 30 ? "°¨Áö" : "-";
    }
}
