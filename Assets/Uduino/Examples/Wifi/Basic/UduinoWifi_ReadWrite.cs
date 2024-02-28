using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Uduino;

#if UDUINO_READY
public class UduinoWifi_ReadWrite : MonoBehaviour
{

    public bool send = false;
    private void Update()
    {

        if(send)
        {
            UduinoManager.Instance.sendCommand("startLoop");
            send = false;
        }
    }


    public void Received(string data, UduinoDevice u)
    {
       Debug.Log(u.name + " " + data);
    }
}

#else
public class UduinoWifi_ReadWrite : MonoBehaviour
{
    [Header("You need to download Uduino first")]
    public string downloadUduino = "https://www.assetstore.unity3d.com/#!/content/78402";
}
#endif

