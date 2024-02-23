using System.Collections;
using System.Collections.Generic;
using RosSharp.RosBridgeClient;
using UnityEngine;

public class RobotView : MonoBehaviour
{
    public GameObject rosConnector;
    public GameObject robotViewPanel;
    private bool isRobotViewActive = false;

    void Start()
    {
        robotViewPanel.SetActive(false);
        rosConnector.GetComponent<ImageSubscriber>().enabled = false;
    }
    
    public void OnClickRobotViewButton()
    {
        if (!isRobotViewActive)
        {
            // ImageSubscriber를 활성화
            rosConnector.GetComponent<ImageSubscriber>().enabled = true;

            // RobotViewPanel 활성화
            robotViewPanel.SetActive(true);

            isRobotViewActive = true;
        }
        else
        {
            CloseRobotView();
        }
    }

    public void CloseRobotView()
    {
        // ImageSubscriber를 비활성화
        rosConnector.GetComponent<ImageSubscriber>().enabled = false;

        // RobotViewPanel 비활성화
        robotViewPanel.SetActive(false);

        isRobotViewActive = false;
    }
}
