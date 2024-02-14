using System;
using UnityEngine;
using UnityEngine.UI;


public class MarkerClickDetector : MonoBehaviour
{
    public GameObject informationPanel; // Ȱ��ȭ�� ���� �г�
    public string markerId; // ���� ��Ŀ�� ID
    public string information; // ���� ��Ŀ�� ����� ����
    private int level;
    private DateTime creationTime;
    public InformationManager informationManager;
    void OnMouseDown()
    {
        if (informationManager != null)
        {
            informationManager.SetSelectedMarkerId(markerId);
            informationManager.DisplayInformation();
            // InformationManager�� ���� Firestore���� ������ ��ȸ�ϰ� UI�� ǥ��
        }
        // ��Ŀ Ŭ�� �� ������ UI�� ǥ��
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
        /*// InformationManager�� ���� ������ UI�� ǥ��
        if (informationManager != null)
        {
            informationManager.DisplayInformation($"Information: {information}\nLevel: {level}\nCreated: {creationTime.ToString("yyyy-MM-dd HH:mm:ss")}");
        }
        else
        {
            Debug.LogError("InformationManager is not assigned.");
        }
*/
        // ���� �г� Ȱ��ȭ
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
