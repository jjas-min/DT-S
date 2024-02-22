using UnityEngine;

public class DashboardController : MonoBehaviour
{
    public GameObject markerPanel;
    public GameObject issuePanel;
    public GameObject DashboardButton;
    public GameObject firstPersonView;
    private bool isDashboardVisible = false;
    // Start is called before the first frame update
    void Start()
    {
        // Dashboard Panel�� ó������ ����
        markerPanel.SetActive(false);
        issuePanel.SetActive(false);
    }

    // Dashboard ��ư�� ���� �� ȣ��Ǵ� �Լ�
    public void OnClickDashboardButton()
    {
        // Dashboard Panel�� ���� ���������� Ȯ��
        if (!isDashboardVisible)
        {
            // Marker Panel�� ������
            isDashboardVisible = true;
            markerPanel.SetActive(true);
            DashboardButton.SetActive(false);
            markerPanel.GetComponent<MarkerDashboard>().WriteDashboard();
            firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = false;
        }
    }

    // Close ��ư�� ������ ȣ��Ǵ� �Լ�
    public void CloseDashboard()
    {
        // Dashboard Panel�� ����
        isDashboardVisible = false;
        markerPanel.SetActive(false);
        issuePanel.SetActive(false);
        DashboardButton.SetActive(true);
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
    }

    // Panel �ٲ��ִ� �Լ�
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