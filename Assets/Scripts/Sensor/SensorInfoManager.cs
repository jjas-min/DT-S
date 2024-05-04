using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO; // íì¼ ì²ë¦¬ë¥¼ ìí ë¤ìì¤íì´ì¤ ì¶ê°

public class SensorInfoManager : MonoBehaviour
{
    [SerializeField] private GameObject sensorInfoPanel;

    private TMP_Text temperatureText; 
    private TMP_Text lightLevelText;
    private TMP_Text waterLevelText;
    private TMP_Text flameDetectedText;
    private TMP_Text humanDetectedText;
    private TMP_Text gasLevelText;
    private TMP_Text pm25LevelText;
    private TMP_Text pm100LevelText;
    
    private TMP_Text timeText;
    private TMP_Text statusText;
    private RawImage statusImage;

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
        gasLevelText = sensorInfoPanel.transform.Find("GasLevel").GetComponent<TMP_Text>();
        pm25LevelText = sensorInfoPanel.transform.Find("PM2.5").GetComponent<TMP_Text>();
        pm100LevelText = sensorInfoPanel.transform.Find("PM10").GetComponent<TMP_Text>();

        timeText = sensorInfoPanel.transform.Find("Current").GetComponent<TMP_Text>();
        statusText = sensorInfoPanel.transform.Find("Status").GetComponent<TMP_Text>();
        statusImage = sensorInfoPanel.transform.Find("RawImage").GetComponent<RawImage>();
        
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

    // íì¼ë¡ë¶í° Texture2Dë¥¼ ë¡ëíë í¨ì
    Texture2D LoadTextureFromFile(string path)
    {
        byte[] fileData = File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2); // Texture2D
        texture.LoadImage(fileData);
        return texture; 
    }

    public void UpdateSensorInformation()
    {
        DateTime currentTime = DateTime.Now;

        sensorPackageIDText.text = sensorData.GetSensorPackageID();
        temperatureText.text = sensorData.GetTemperature() != null ? $"{sensorData.GetTemperature():F0}°" : "-";
        lightLevelText.text = sensorData.GetLightLevel() != null ? $"조도: {sensorData.GetLightLevel()}" : "조도: -";
        waterLevelText.text = sensorData.GetWaterLevel() != null ? $"수위: {sensorData.GetWaterLevel()}" : "수위: -";
        flameDetectedText.text = sensorData.GetFlameDetected() != null ? $"불꽃감지: {sensorData.GetFlameDetected()}" : "불꽃감지: -";
        humanDetectedText.text = sensorData.GetHumanDetected() > 30 ? "인체감지: 감지" : "인체감지: -";
        gasLevelText.text = sensorData.GetGasLevel() != null ? $"일산화탄소: {sensorData.GetGasLevel()}" : "일산화탄소: -";
        pm25LevelText.text = sensorData.GetPM25Level() != null ? $"PM2.5: {sensorData.GetPM25Level()}" : "PM2.5: -";
        pm100LevelText.text = sensorData.GetPM100Level() != null ? $"PM10: {sensorData.GetPM100Level()}" : "PM10: -";

        string timeString = currentTime.ToLocalTime().ToString("hh:mm tt");
        timeText.text = timeString;

        string imagePath;
            Color color;

            if (sensorData.GetFlameDetected() > 20 || sensorData.GetTemperature() > 80)
            {
                statusText.text = "위험";
                if (ColorUtility.TryParseHtmlString("#FF0000", out color))
                {
                    statusText.color = color;
                }
                imagePath = Application.dataPath + "/Images/red.png";
            }
            else if (sensorData.GetTemperature() > 50 || sensorData.GetHumanDetected() == 1)
            {
                statusText.text = "주의";
                if (ColorUtility.TryParseHtmlString("#FFEB40", out color))
                {
                    statusText.color = color;
                }
                imagePath = Application.dataPath + "/Images/warning.png";
            }
            else
            {
                statusText.text = "안전";
                if (ColorUtility.TryParseHtmlString("#38D800", out color))
                {
                    statusText.color = color;
                }
                imagePath = Application.dataPath + "/Images/safe.png";
            }

            temperatureText.color = color;
            Texture2D texture = LoadTextureFromFile(imagePath);
            statusImage.texture = texture;
    }
}
