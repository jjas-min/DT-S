using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MarkerDashboard : MonoBehaviour
{
    public TMP_Text textDisplay; // TextMeshPro에 표시할 텍스트 UI 요소
    public MarkerData[] markerDataArray;
    
    private void WriteDashboard() {
        // MarkerData Array
        MarkerData[] markerDataArray = FindObjectsOfType<MarkerData>();

        // Location으로 그룹화하여 저장할 딕셔너리
        Dictionary<string, List<MarkerData>> locationGroups = new Dictionary<string, List<MarkerData>>();

        // 각 MarkerData를 Location으로 그룹화
        foreach (MarkerData markerData in markerDataArray)
        {
            if (!locationGroups.ContainsKey(markerData.location))
            {
                locationGroups[markerData.location] = new List<MarkerData>();
            }
            locationGroups[markerData.location].Add(markerData);
        }

        // 표시할 텍스트 초기화
        string displayText = "";

        // Location 그룹별로 정보 추가
        foreach (var locationGroup in locationGroups)
        {
            displayText += "<color=#000000>Location: " + locationGroup.Key + "</color>\n";
            displayText += "<color=#000000>============</color>\n";

            foreach (MarkerData markerData in locationGroup.Value)
            {
                displayText += "ID: " + markerData.id + "\n";
                displayText += "Level: " + markerData.level + "\n";
                displayText += "Information: " + markerData.information + "\n";
                displayText += "Creation Time: " + markerData.creationTime.ToString("yyyy-MM-dd HH:mm:ss") + "\n";
                displayText += "-----------------\n";
            }

            displayText += "\n"; // Location 그룹 간 간격 추가
        }

        // TextMeshPro UI 요소에 displayText를 할당합니다.
        textDisplay.text = displayText;
        // 텍스트 크기를 20포인트로 설정합니다.
        textDisplay.fontSize = 20;
    }
}
