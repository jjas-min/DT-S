using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorInfoPanelController : MonoBehaviour
{
    [SerializeField] private GameObject sensorInfoPanel;

    [SerializeField] private string activeSensorPackageID;

    public void OnCloseButtonClicked()
    {
        sensorInfoPanel.SetActive(false); // X ��ư Ŭ�� �� Information Panel ��Ȱ��ȭ
    }

    public void SetActiveSensorPackageID(string sensorPackageID)
    {
        activeSensorPackageID = sensorPackageID;
    }

    public string GetActiveSensorPackageID()
    {
        return activeSensorPackageID;
    }
}
