using System.Collections;
using System.Collections.Generic;
using Uduino;
using UnityEngine;
using UnityEngine.UI;

public class UduinoWifiInterfaceStart : MonoBehaviour
{
    public InputField IpText;
    public InputField portText;

 
    public void ConnectWifi()
    {
        UduinoWiFiSettings u = new UduinoWiFiSettings();
        u.ip = IpText.text;
        u.port = int.Parse(portText.text);
        UduinoManager.Instance.UduinoWiFiBoards.Add(u);

        UduinoManager.Instance.DiscoverPorts();
    }

    public void LoadWifi()
    {
        if(PlayerPrefs.HasKey("Uduino_Wifi_IP"))
        {
            IpText.text = PlayerPrefs.GetString("Uduino_Wifi_IP");
        }


        if (PlayerPrefs.HasKey("Uduino_Wifi_PORT"))
        {
            portText.text = PlayerPrefs.GetString("Uduino_Wifi_PORT");
        }
    }

    public void SaveWifi()
    {
        PlayerPrefs.SetString("Uduino_Wifi_IP", IpText.text);
        PlayerPrefs.SetString("Uduino_Wifi_PORT", portText.text);
    }

 
}
