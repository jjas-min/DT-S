using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq; // LINQ 라이브러리 추가

public class MarkerDashboard : MonoBehaviour
{
    public RectTransform content; // 스크롤 뷰의 Content 오브젝트
    public GameObject panelPrefab; // 이미지와 텍스트를 담을 패널 프리팹
    public TMP_FontAsset font; // 사용할 폰트
    public Color fontColor = Color.black; // 폰트 컬러
    public float verticalSpacing = 10f; // 이미지들 사이의 수직 간격
    private MarkerData[] markerDataArray;

    public void WriteDashboard()
    {
        // MarkerData 배열을 creationTime을 기준으로 정렬
        markerDataArray = FindObjectsOfType<MarkerData>().OrderBy(markerData => markerData.creationTime).ToArray();

        // 스크롤 뷰의 Content 초기화
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        // 각 마커 데이터에 대해 이미지와 텍스트를 담을 패널 생성 및 정보 표시
        foreach (MarkerData markerData in markerDataArray)
        {
            // 패널 생성
            GameObject panelObject = Instantiate(panelPrefab, content);
            
            // 텍스트 생성 및 설정
            TMP_Text markerText = panelObject.GetComponentInChildren<TMP_Text>();
            markerText.font = font; // 폰트 설정
            markerText.color = fontColor;
            markerText.fontSize = 20;
            markerText.text = $"ID: {markerData.id}\n" +
                              $"Information: {markerData.information}\n" +
                              $"Level: {markerData.level}\n" +
                              $"Location: {markerData.location}\n" +
                              $"Creation Time: {markerData.creationTime.ToString("yyyy-MM-dd HH:mm:ss")}";
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