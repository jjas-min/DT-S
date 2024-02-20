using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MarkerGroup : MonoBehaviour
{
    public TMP_Text textDisplay; // TextMeshPro를 사용하기 위한 UI 요소
    private string displayText;

    void Start()
    {
        // MarkerData 컴포넌트를 가진 모든 게임 오브젝트를 찾습니다.
        MarkerData[] markerDataArray = FindObjectsOfType<MarkerData>();

        // Location별로 정보를 그룹화하는 딕셔너리 생성
        Dictionary<string, List<MarkerData>> locationGroups = new Dictionary<string, List<MarkerData>>();

        // 각 MarkerData를 Location별로 그룹화
        foreach (MarkerData markerData in markerDataArray)
        {
            if (!locationGroups.ContainsKey(markerData.location))
            {
                locationGroups[markerData.location] = new List<MarkerData>();
            }
            locationGroups[markerData.location].Add(markerData);
        }

        // 내용을 저장할 문자열 변수
        displayText = "";

        // Location 그룹마다 정보를 추가
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

            displayText += "\n"; // Location 그룹 사이에 빈 줄 추가
        }

        PrintGroup();
    }

    private void PrintGroup()
    {
        // TextMeshPro UI 요소에 displayText를 설정합니다.
        textDisplay.text = this.displayText;
        // 폰트 크기를 20으로 설정합니다.
        textDisplay.fontSize = 20;
    }
}
