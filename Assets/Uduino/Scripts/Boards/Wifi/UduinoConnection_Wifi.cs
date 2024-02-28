using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UDUINO_READY
using System.IO.Ports;
#endif

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Uduino
{
#if UDUINO_READY

    public class UduinoConnection_Wifi : UduinoConnection
    {
        // public List<BoardPortSettings> boardSetting = new List<BoardPortSettings>();
        public Dictionary<UduinoDevice, UduinoWiFiSettings> settingsDic = new Dictionary<UduinoDevice, UduinoWiFiSettings>();

        public UduinoConnection_Wifi() : base()
        {
           // StartNetwork();
        }

        public override void FindBoards(UduinoManager manager)
        {
            base.FindBoards(manager);
            Discover();
        }

        public override void Discover()
        {
            //for (int i = 0; i < UduinoManager.Instance.UduinoWiFiBoards.Count; i++)
            foreach (UduinoWiFiSettings setting in UduinoManager.Instance.UduinoWiFiBoards)
           {
                UduinoDevice tmpDevice = new UduinoDevice_Wifi(this, setting); //OpenUduinoDevice(boardSet.wifiBoardSetting.ip);
                tmpDevice.Open();
                DetectUduino(tmpDevice);
            }
        }

        public override void Stop()
        {
            /*
            for (int i=0; i< boardSetting.Count; i++)
            {
                boardSetting[i].udpClient.Close();
            }
            boardSetting.Clear();*/
        }
    }

#else
    public class UduinoConnection_Wifi : MonoBehaviour
    {
        [Header("You need to download Uduino first")]
        public string downloadUduino = "https://www.assetstore.unity3d.com/#!/content/78402";
    }
#endif
}