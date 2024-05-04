using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq; // LINQ 라이브러리 추가
using System.IO; // 파일 처리를 위한 네임스페이스 추가

public class SensorDashboard : MonoBehaviour
{
    public RectTransform content;
    public GameObject panelPrefab; // 이미지와 텍스트를 담을 패널 프리팹
    public TMP_FontAsset font; // 사용할 폰트
    public Color fontColor = Color.black; // 폰트 컬러
    public float verticalSpacing = 10f; // 이미지들 사이의 수직 간격
    public SensorData[] SensorDataArray;

    // 파일로부터 Texture2D를 로드하는 함수
    Texture2D LoadTextureFromFile(string path)
    {
        byte[] fileData = File.ReadAllBytes(path); // 파일을 바이트 배열로 읽기
        Texture2D texture = new Texture2D(2, 2); // Texture2D 객체 생성
        texture.LoadImage(fileData); // 바이트 배열을 이미지로 로드
        return texture; // 로드한 이미지 반환
    }

    public void WriteDashboard()
    {
        // MarkerData Array
        SensorDataArray = FindObjectsOfType<SensorData>();
        DateTime currentTime = DateTime.Now;

        // 스크롤 뷰의 Content 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 각 센서 데이터에 대해 이미지와 텍스트를 담을 패널 생성 및 정보 표시
        foreach (SensorData sensorData in SensorDataArray)
        {
            // 패널 생성
            GameObject panelObject = Instantiate(panelPrefab, content);

            // // 패널에서 필요한 텍스트 컴포넌트를 찾아 정보 설정
            TMP_Text titleText = panelObject.transform.Find("Title").GetComponent<TMP_Text>();
            titleText.text = sensorData.GetSensorPackageID();
            
            TMP_Text timeText = panelObject.transform.Find("Current").GetComponent<TMP_Text>();
            string timeString = currentTime.ToLocalTime().ToString("hh:mm tt");
            timeText.text = timeString;

            TMP_Text temperatureText = panelObject.transform.Find("Temperature").GetComponent<TMP_Text>();
            string temperatureString = sensorData.GetTemperature() != null ? $"{sensorData.GetTemperature():F0}°" : "-";
            temperatureText.text = temperatureString;

            TMP_Text waterLevelText = panelObject.transform.Find("WaterLevel").GetComponent<TMP_Text>();
            waterLevelText.text = sensorData.GetWaterLevel() != null ? $"수위: {sensorData.GetWaterLevel()}" : "수위: -";

            TMP_Text lightText = panelObject.transform.Find("Light").GetComponent<TMP_Text>();
            lightText.text = sensorData.GetLightLevel() != null ? $"조도: {sensorData.GetLightLevel()}" : "조도: -";

            TMP_Text flameDetectedText = panelObject.transform.Find("FlameDetected").GetComponent<TMP_Text>();
            flameDetectedText.text = sensorData.GetFlameDetected() != null ? $"불꽃감지: {sensorData.GetFlameDetected()}" : "불꽃감지: -";

            TMP_Text humanDetectedText = panelObject.transform.Find("HumanDetected").GetComponent<TMP_Text>();
            humanDetectedText.text = (sensorData.GetHumanDetected() != null && sensorData.GetHumanDetected() > 30) ? $"인체감지: 감지" : "인체감지: -";

            //TMP_Text gasLevelText = panelObject.transform.Find("GasLevel").GetComponent<TMP_Text>();
            //gasLevelText.text = sensorData.GetGasLevel() != null ? $"일산화탄소: {sensorData.GetGasLevel()}" : "일산화탄소: -";

            TMP_Text statusText = panelObject.transform.Find("Status").GetComponent<TMP_Text>();
            RawImage statusImage = panelObject.transform.Find("RawImage").GetComponent<RawImage>();

            string imagePath;
            Color color;

            // 불꽃 감지 여부에 따라 상태 텍스트 설정
            if (sensorData.GetFlameDetected() > 15 || sensorData.GetTemperature() > 30)
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

        // Vertical Layout Group 컴포넌트의 child force expand를 적용하여 공백을 생성
        VerticalLayoutGroup verticalLayoutGroup = content.GetComponent<VerticalLayoutGroup>();
        if (verticalLayoutGroup != null)
        {
            verticalLayoutGroup.childForceExpandHeight = true;
            verticalLayoutGroup.spacing = verticalSpacing;
        }
    }
}
