using UnityEngine;

public class MarkerClickDetector : MonoBehaviour
{
    public GameObject informationPanel; // Ȱ��ȭ�� ���� �г�
    public string markerId; // ���� ��Ŀ�� ID
    public string information; // ���� ��Ŀ�� ����� ����
    public InformationManager informationManager;

    void OnMouseDown()
    {
        if (informationPanel != null)
        {
            informationManager.SetSelectedMarkerId(this.markerId);
            informationPanel.SetActive(true);
 
            if (informationManager != null)
            {
                informationManager.DisplayInformation(information); // ���� ǥ�� �޼��� ȣ��
            }
        }
    }
}