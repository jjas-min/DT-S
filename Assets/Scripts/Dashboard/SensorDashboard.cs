using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; // LINQ 라이브러리 추가

public class SensorDashboard : MonoBehaviour
{
    public RectTransform content;
    public GameObject panelPrefab; // 이미지와 텍스트를 담을 패널 프리팹
    public TMP_FontAsset font; // 사용할 폰트
    public Color fontColor = Color.black; // 폰트 컬러
    public Texture2D imageTexture; // RawImage에 사용할 이미지 텍스처
    public SensorData[] SensorDataArray;

    public void WriteDashboard() 
    {
        // MarkerData Array
        SensorDataArray = FindObjectsOfType<SensorData>();

        // 스크롤 뷰의 Content 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }
        
        // 각 센서 데이터에 대해 이미지와 텍스트를 담을 패널 생성 및 정보 표시
        foreach (SensorData sensorData in SensorDataArray)
        {
            Debug.Log("inside foreachinside foreachinside foreachinside foreachinside foreachinside foreachinside foreachinside foreachinside foreachinside foreach");
            
            // 패널 생성
            GameObject panelObject = Instantiate(panelPrefab, content);
            
            // 텍스트 컴포넌트를 찾아서 정보 설정
            TMP_Text idText = panelObject.transform.Find("Title").GetComponent<TMP_Text>();
            TMP_Text temperatureText = panelObject.transform.Find("Temperature").GetComponent<TMP_Text>();
            TMP_Text waterLevelText = panelObject.transform.Find("WaterLevel").GetComponent<TMP_Text>();
            TMP_Text flameDetectedText = panelObject.transform.Find("FlameDetected").GetComponent<TMP_Text>();
            TMP_Text humanDetectedText = panelObject.transform.Find("HumanDetected").GetComponent<TMP_Text>();

            // RawImage 컴포넌트를 찾아서 이미지 설정
            RawImage rawImageComponent = panelObject.transform.Find("RawImage").GetComponent<RawImage>();
            Debug.Log("rawImageComponent", rawImageComponent);
            rawImageComponent.texture = imageTexture;

            // 각 텍스트 필드에 정보 할당
            idText.font = font; // 폰트 설정
            idText.color = fontColor;
            idText.fontSize = 15;
            idText.text = sensorData.GetSensorPackageID();
            
            temperatureText.font = font; // 폰트 설정
            temperatureText.color = fontColor;
            temperatureText.fontSize = 45;
            temperatureText.text = $"{sensorData.GetLightLevel()}°C";
            
            waterLevelText.font = font; // 폰트 설정
            waterLevelText.color = fontColor;
            waterLevelText.fontSize = 15;
            waterLevelText.text = $"수위 정보: {sensorData.GetWaterLevel()}";
            
            flameDetectedText.font = font; // 폰트 설정
            flameDetectedText.color = fontColor;
            flameDetectedText.fontSize = 15;
            flameDetectedText.text = $"불꽃 감지: {sensorData.GetFlameDetected()}";
            
            humanDetectedText.font = font; // 폰트 설정
            humanDetectedText.color = fontColor;
            humanDetectedText.fontSize = 15;
            humanDetectedText.text = $"사람 감지: {sensorData.GetHumanDetected()}";
        }
    }
}
