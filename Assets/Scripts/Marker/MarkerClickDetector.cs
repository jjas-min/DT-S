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
    private string location;
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
    public void SetMarkerDetails(string information, int level, DateTime creationTime, string location)
    {
        this.information = information;
        this.level = level;
        this.creationTime = creationTime;
        this.location = location;
    }
    private void DisplayInformation()
    {
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
