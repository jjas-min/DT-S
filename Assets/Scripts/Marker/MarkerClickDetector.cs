using System;
using UnityEngine;
using UnityEngine.UI;


public class MarkerClickDetector : MonoBehaviour
{
    public GameObject informationPanel; // 활성화할 정보 패널
    public string markerId; // 현재 마커의 ID
    public string information; // 현재 마커에 저장된 정보
    private int level;
    private DateTime creationTime;
    public InformationManager informationManager;
    void OnMouseDown()
    {
        if (informationManager != null)
        {
            informationManager.SetSelectedMarkerId(markerId);
            informationManager.DisplayInformation();
            // InformationManager를 통해 Firestore에서 정보를 조회하고 UI에 표시
        }
        // 마커 클릭 시 정보를 UI에 표시
        DisplayInformation();
    }
    public void SetMarkerDetails(string information, int level, DateTime creationTime)
    {
        this.information = information;
        this.level = level;
        this.creationTime = creationTime;
    }
    private void DisplayInformation()
    {
        /*// InformationManager를 통해 정보를 UI에 표시
        if (informationManager != null)
        {
            informationManager.DisplayInformation($"Information: {information}\nLevel: {level}\nCreated: {creationTime.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        else
        {
            Debug.LogError("InformationManager is not assigned.");
        }
*/
        // 정보 패널 활성화
        if (informationPanel != null)
        {
            informationPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Information Panel is not assigned.");
        }
    }
}
