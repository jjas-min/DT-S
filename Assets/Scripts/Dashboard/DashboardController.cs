using UnityEngine;

public class DashboardController : MonoBehaviour
{
    public GameObject markerPanel;
    public GameObject sensorPanel;
    public GameObject firstPersonView;

    // Start is called before the first frame update
    void Start()
    {
        // Dashboard Panel�� ó������ ����
        markerPanel.SetActive(false);
        sensorPanel.SetActive(false);
    }

    // Dashboard ��ư�� ���� �� ȣ��Ǵ� �Լ�
    public void OnClickMarkerDashboardButton()
    {
        // If marker dashboard is not active
        if (!markerPanel.activeSelf)
        {
            // Activate marker dashboard
            markerPanel.SetActive(true);
            sensorPanel.SetActive(false);
            markerPanel.GetComponent<MarkerDashboard>().WriteDashboard();
            firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = false;
        }
        else
        {
            // Deactivate marker dashboard
            markerPanel.SetActive(false);
            firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
        }
    }

    public void OnClickSensorDashboardButton()
    {
        // If sensor dashboard is not active
        if (!sensorPanel.activeSelf)
        {
            // Activate sensor dashboard
            sensorPanel.SetActive(true);
            markerPanel.SetActive(false);
            sensorPanel.GetComponent<SensorDashboard>().WriteDashboard();
            firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = false;
        }
        else
        {
            // Deactivate sensor dashboard
            sensorPanel.SetActive(false);
            firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
        }
    }

    // Close ��ư�� ������ ȣ��Ǵ� �Լ�
    public void OnClickCloseDashboard()
    {
        markerPanel.SetActive(false);
        sensorPanel.SetActive(false);
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
    }
}