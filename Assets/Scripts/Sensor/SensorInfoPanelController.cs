using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SensorInfoPanelController : MonoBehaviour
{
    [SerializeField] private GameObject sensorInfoPanel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCloseButtonClicked()
    {
        sensorInfoPanel.SetActive(false); // X ��ư Ŭ�� �� Information Panel ��Ȱ��ȭ
    }
}
