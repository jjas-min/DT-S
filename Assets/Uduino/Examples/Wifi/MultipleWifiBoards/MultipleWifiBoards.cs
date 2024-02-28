using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Uduino;


//  How to work with this example:
// 1. Upload FirstBoard.ino to your first esp8266 boards and SecondBoard.ino to your second. The port should be changed in the second board. 
// 2. Add the IP address the correct port in UduinoManager
// 3. Press Play. You r boards should be detected!


public class MultipleWifiBoards : MonoBehaviour
{
    public bool sendCommandToFirstArduino = false;

    private void Update()
    {
        if (sendCommandToFirstArduino)
        {
            UduinoDevice firstBoard = UduinoManager.Instance.GetBoard("firstBoard");
            UduinoManager.Instance.sendCommand(firstBoard, "startLoop");
            sendCommandToFirstArduino = false;
        }
    }

    public void Received(string data, UduinoDevice u)
    {
        if (u.name == "firstBoard")
        {
            Debug.Log("Receiving: " + data + " from " + u.name);
        }
        else
        {
            Debug.Log("Receiving: " + data + " from " + u.name);
        }
    }
}