using UnityEngine;


public class MarkerClickDetector : MonoBehaviour
{
    public MarkerInfoManager markerInfoManager;

    void OnMouseDown()
    {
        MarkerData markerData = GetComponent<MarkerData>();
        if (markerData != null && markerInfoManager != null)
        {
            markerInfoManager.DisplayInformation(markerData);
        }
    }
}