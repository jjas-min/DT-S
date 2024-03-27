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
    public float verticalSpacing = 10f; // 이미지들 사이의 수직 간격
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
            // 패널 생성
            GameObject paenlObject = Instantiate(panelPrefab, content);
            
            // 텍스트 컴포넌트를 찾아서 텍스트 설정
            TMP_Text markerText = paenlObject.GetComponentInChildren<TMP_Text>();
            markerText.font = font; // 폰트 설정
            markerText.color = fontColor;
            markerText.fontSize = 15;
            markerText.text = $"ID: {sensorData.GetSensorPackageID()}\n" +
                            $"Temperature: {sensorData.GetLightLevel()}\n" +
                            $"WaterLevel: {sensorData.GetWaterLevel()}\n" +
                            $"FlameDetected: {sensorData.GetFlameDetected()}\n" +
                            $"HumanDetected: {sensorData.GetHumanDetected()}\n";
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