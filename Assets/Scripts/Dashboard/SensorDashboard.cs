using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SensorDashboard : MonoBehaviour
{
    public RectTransform content;
    public GameObject textPrefab; // 텍스트를 생성할 프리팹
    public SensorData[] SensorDataArray;

    public void WriteDashboard() {
        // MarkerData Array
        SensorDataArray = FindObjectsOfType<SensorData>();
        Debug.Log(SensorDataArray.Length);
        foreach (SensorData sensorData in SensorDataArray)
        {
            GameObject textObject = Instantiate(textPrefab, content);
            SetSensorDataOnText(textObject, sensorData);
        }
    }

    private void SetSensorDataOnText(GameObject textObject, SensorData sensorData)
    {
        // 텍스트 컴포넌트를 찾아서 텍스트 설정
        TMP_Text markerText = textObject.GetComponent<TMP_Text>();
        markerText.fontSize = 20;
        markerText.text = $"ID: {sensorData.GetSensorPackageID()}\n" +
                          $"Temperature: {sensorData.GetLightLevel()}\n" +
                          $"WaterLevel: {sensorData.GetWaterLevel()}\n" +
                          $"FlameDetected: {sensorData.GetFlameDetected()}\n" +
                          $"HumanDetected: {sensorData.GetHumanDetected()}\n";

        markerText.enabled = true;
    }
}
