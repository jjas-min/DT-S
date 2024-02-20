using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq; // LINQ 라이브러리 추가

public class MarkerDashboard : MonoBehaviour
{
    public RectTransform content; // 스크롤 뷰의 Content 오브젝트
    public GameObject textPrefab; // 텍스트를 생성할 프리팹
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

        // 각 마커 데이터에 대해 텍스트 생성 및 정보 표시
        foreach (MarkerData markerData in markerDataArray)
        {
            // 텍스트 생성
            GameObject textObject = Instantiate(textPrefab, content);
            textObject.name = markerData.id.ToString();

            // 텍스트에 마커 데이터 정보 표시
            SetMarkerDataOnText(textObject, markerData);
        }
    }

    private void SetMarkerDataOnText(GameObject textObject, MarkerData markerData)
    {
        // 텍스트 컴포넌트를 찾아서 텍스트 설정
        TMP_Text markerText = textObject.GetComponent<TMP_Text>();
        markerText.fontSize = 20;
        markerText.text = $"ID: {markerData.id}\n" +
                          $"Information: {markerData.information}\n" +
                          $"Level: {markerData.level}\n" +
                          $"Location: {markerData.location}\n" +
                          $"Position: {markerData.position}\n" +
                          $"Creation Time: {markerData.creationTime.ToString("yyyy-MM-dd HH:mm:ss")}\n";
        
        markerText.enabled = true;
    }
}
