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
        //gasLevelText = sensorInfoPanel.transform.Find("GasLevel").GetComponent<TMP_Text>();
       
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
        byte[] fileData = File.ReadAllBytes(path); // íì¼ì ë°ì´í¸ ë°°ì´ë¡ ì½ê¸°
        Texture2D texture = new Texture2D(2, 2); // Texture2D ê°ì²´ ìì±
        texture.LoadImage(fileData); // ë°ì´í¸ ë°°ì´ì ì´ë¯¸ì§ë¡ ë¡ë
        return texture; // ë¡ëí ì´ë¯¸ì§ ë°í
    }

    public void UpdateSensorInformation()
    {
        DateTime currentTime = DateTime.Now;

        sensorPackageIDText.text = sensorData.GetSensorPackageID();
        temperatureText.text = $"{sensorData.GetTemperature():F0}Â°";
        lightLevelText.text = $"ì¡°ë: {sensorData.GetLightLevel()}";
        waterLevelText.text = $"ìì: {sensorData.GetWaterLevel()}";
        flameDetectedText.text = $"ë¶ê½ê°ì§: {sensorData.GetFlameDetected()}";
        humanDetectedText.text = sensorData.GetHumanDetected() > 30 ? "ì¬ëê°ì§: ê°ì§" : "ì¬ëê°ì§: -";
        gasLevelText.text = $"{sensorData.GetGasLevel()}";

        string timeString = currentTime.ToLocalTime().ToString("hh:mm tt");
        timeText.text = timeString;

        string imagePath;
            Color color;

            // ë¶ê½ ê°ì§ ì¬ë¶ì ë°ë¼ ìí íì¤í¸ ì¤ì 
            if (sensorData.GetFlameDetected() > 20 || sensorData.GetTemperature() > 80)
            {
                statusText.text = "ìí";
                if (ColorUtility.TryParseHtmlString("#FF0000", out color))
                {
                    statusText.color = color;
                }
                imagePath = Application.dataPath + "/Images/red.png";
            }
            else if (sensorData.GetTemperature() > 50 || sensorData.GetHumanDetected() == 1)
            {
                statusText.text = "ê²½ê³ ";
                if (ColorUtility.TryParseHtmlString("#FFEB40", out color))
                {
                    statusText.color = color;
                }
                imagePath = Application.dataPath + "/Images/warning.png";
            }
            else
            {
                statusText.text = "ìí¸";
                if (ColorUtility.TryParseHtmlString("#38D800", out color))
                {
                    statusText.color = color;
                }
                imagePath = Application.dataPath + "/Images/safe.png";
            }

            temperatureText.color = color;
            Texture2D texture = LoadTextureFromFile(imagePath); // ì´ë¯¸ì§ ë¡ë
            statusImage.texture = texture; // ì´ë¯¸ì§ í ë¹
    }
}
