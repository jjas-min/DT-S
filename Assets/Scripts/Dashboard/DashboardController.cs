using UnityEngine;

public class DashboardController : MonoBehaviour
{
    public GameObject markerPanel;
    public GameObject issuePanel;
    private bool isDashboardVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        // Dashboard Panel을 처음에는 숨김
        markerPanel.SetActive(false);
        issuePanel.SetActive(false);
    }

    // Dashboard 버튼을 누를 때 호출되는 함수
    public void OnClickDashboardButton()
    {
        // Dashboard Panel이 현재 보여지는지 확인
        if (!isDashboardVisible)
        {
            // Marker Panel을 보여줌
            isDashboardVisible = true;
            markerPanel.SetActive(true);
        }
    }

    // Close 버튼을 누르면 호출되는 함수
    public void CloseDashboard()
    {
        // Dashboard Panel을 숨김
        isDashboardVisible = false;
        markerPanel.SetActive(false);
        issuePanel.SetActive(false);
    }

    // Panel 바꿔주는 함수
    public void ToggleDashboard()
    {
        // Toggle Panel
        if (markerPanel.activeSelf)
        {
            markerPanel.SetActive(false);
            issuePanel.SetActive(true);
        }
        else
        {
            issuePanel.SetActive(false);
            markerPanel.SetActive(true);
        }
    }
}