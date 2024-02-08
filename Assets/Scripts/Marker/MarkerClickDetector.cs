using UnityEngine;

public class MarkerClickDetector : MonoBehaviour
{
    public GameObject informationPanel; // 활성화할 정보 패널
    public string markerId; // 현재 마커의 ID
    public string information; // 현재 마커에 저장된 정보
    public InformationManager informationManager;

    void OnMouseDown()
    {
        if (informationPanel != null)
        {
            informationManager.SetSelectedMarkerId(this.markerId);
            informationPanel.SetActive(true);
 
            if (informationManager != null)
            {
                informationManager.DisplayInformation(information); // 정보 표시 메서드 호출
            }
        }
    }
}