using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MarkerGroup : MonoBehaviour
{
    public TMP_Text textDisplay; // TextMeshPro�� ����ϱ� ���� UI ���
    private string displayText;

    void Start()
    {
        // MarkerData ������Ʈ�� ���� ��� ���� ������Ʈ�� ã���ϴ�.
        MarkerData[] markerDataArray = FindObjectsOfType<MarkerData>();

        // Location���� ������ �׷�ȭ�ϴ� ��ųʸ� ����
        Dictionary<string, List<MarkerData>> locationGroups = new Dictionary<string, List<MarkerData>>();

        // �� MarkerData�� Location���� �׷�ȭ
        foreach (MarkerData markerData in markerDataArray)
        {
            if (!locationGroups.ContainsKey(markerData.location))
            {
                locationGroups[markerData.location] = new List<MarkerData>();
            }
            locationGroups[markerData.location].Add(markerData);
        }

        // ������ ������ ���ڿ� ����
        displayText = "";

        // Location �׷츶�� ������ �߰�
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

            displayText += "\n"; // Location �׷� ���̿� �� �� �߰�
        }

        PrintGroup();
    }

    private void PrintGroup()
    {
        // TextMeshPro UI ��ҿ� displayText�� �����մϴ�.
        textDisplay.text = this.displayText;
        // ��Ʈ ũ�⸦ 20���� �����մϴ�.
        textDisplay.fontSize = 20;
    }
}
