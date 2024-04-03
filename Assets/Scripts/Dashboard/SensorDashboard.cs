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
            GameObject panelObject = Instantiate(panelPrefab, content);

            // // 패널에서 필요한 텍스트 컴포넌트를 찾아 정보 설정
            TMP_Text titleText = panelObject.transform.Find("Title").GetComponent<TMP_Text>();
            titleText.text = sensorData.GetSensorPackageID();

            TMP_Text temperatureText = panelObject.transform.Find("Temperature").GetComponent<TMP_Text>();
            temperatureText.text = sensorData.GetLightLevel() != null ? $"{sensorData.GetLightLevel()}°C" : "-";

            TMP_Text waterLevelText = panelObject.transform.Find("WaterLevel").GetComponent<TMP_Text>();
            waterLevelText.text = sensorData.GetWaterLevel() != null ? $"수위정보: {sensorData.GetWaterLevel()}" : "수위정보: -";

            TMP_Text flameDetectedText = panelObject.transform.Find("FlameDetected").GetComponent<TMP_Text>();
            flameDetectedText.text = sensorData.GetFlameDetected() != null ? $"불꽃감지: {sensorData.GetFlameDetected()}" : "불꽃감지: -";

            TMP_Text humanDetectedText = panelObject.transform.Find("HumanDetected").GetComponent<TMP_Text>();
            humanDetectedText.text = sensorData.GetHumanDetected() != null ? $"인체감지: {sensorData.GetHumanDetected()}" : "인체감지: -";
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
