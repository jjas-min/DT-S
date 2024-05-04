using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.IO; // 파일 처리를 위한 네임스페이스 추가

public class SensorInfoManager : MonoBehaviour
{
    [SerializeField] private GameObject sensorInfoPanel;

    private TMP_Text temperatureText; 
    private TMP_Text lightLevelText;
    private TMP_Text waterLevelText;
    private TMP_Text flameDetectedText;
    private TMP_Text humanDetectedText;
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

    // 파일로부터 Texture2D를 로드하는 함수
    Texture2D LoadTextureFromFile(string path)
    {
        byte[] fileData = File.ReadAllBytes(path); // 파일을 바이트 배열로 읽기
        Texture2D texture = new Texture2D(2, 2); // Texture2D 객체 생성
        texture.LoadImage(fileData); // 바이트 배열을 이미지로 로드
        return texture; // 로드한 이미지 반환
    }

    public void UpdateSensorInformation()
    {
        DateTime currentTime = DateTime.Now;

        sensorPackageIDText.text = sensorData.GetSensorPackageID();
        temperatureText.text = $"{sensorData.GetTemperature():F0}°";
        lightLevelText.text = $"조도: {sensorData.GetLightLevel()}";
        waterLevelText.text = $"수위: {sensorData.GetWaterLevel()}";
        flameDetectedText.text = $"불꽃감지: {sensorData.GetFlameDetected()}";
        humanDetectedText.text = sensorData.GetHumanDetected() > 30 ? "사람감지: 감지" : "사람감지: -";

        string timeString = currentTime.ToLocalTime().ToString("hh:mm tt");
        timeText.text = timeString;

        string imagePath;
            Color color;

            // 불꽃 감지 여부에 따라 상태 텍스트 설정
            if (sensorData.GetFlameDetected() > 20 || sensorData.GetTemperature() > 80)
            {
                statusText.text = "위험";
                if (ColorUtility.TryParseHtmlString("#FF0000", out color))
                {
                    statusText.color = color;
                }
                imagePath = Application.dataPath + "/Images/red.png";
            }
            else if (sensorData.GetTemperature() > 50 || sensorData.GetHumanDetected() == 1)
            {
                statusText.text = "경고";
                if (ColorUtility.TryParseHtmlString("#FFEB40", out color))
                {
                    statusText.color = color;
                }
                imagePath = Application.dataPath + "/Images/warning.png";
            }
            else
            {
                statusText.text = "양호";
                if (ColorUtility.TryParseHtmlString("#38D800", out color))
                {
                    statusText.color = color;
                }
                imagePath = Application.dataPath + "/Images/safe.png";
            }

            temperatureText.color = color;
            Texture2D texture = LoadTextureFromFile(imagePath); // 이미지 로드
            statusImage.texture = texture; // 이미지 할당
    }
}
