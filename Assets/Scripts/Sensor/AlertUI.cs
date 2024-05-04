using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro;

public class SensorAlert : MonoBehaviour
{
    public GameObject alertsUI;
    public GameObject alertPanelPrefab; 
    public GameObject firstPersonView;
    public Transform alertsPanel; 
    private FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = false;
        ListenAlerts();
    }

    void ListenAlerts()
    {
        db.Collection("IssueAlerts").OrderByDescending("createdTime").Listen(snapshot =>
        {
            foreach (var change in snapshot.GetChanges())
            {
                if (change.ChangeType == DocumentChange.Type.Added)
                {
                    Debug.Log($"New Alert: {change.Document.Id}");
                    NewAlert(change.Document);
                }
                else if (change.ChangeType == DocumentChange.Type.Removed)
                {
                    Debug.Log($"Alert removed: {change.Document.Id}");
                    RemoveAlert(change.Document.Id);
                }
            }
        });
    }

    void NewAlert(DocumentSnapshot document)
    {
        if (!document.Exists) return;
        Dictionary<string, object> alertData = document.ToDictionary();

        string formattedTime = TimeZoneInfo.ConvertTimeFromUtc(((Timestamp)alertData["createdTime"]).ToDateTime(), TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time")).ToString("yyyy-MM-dd hh:mm tt");

        GameObject alertPanelInstance = Instantiate(alertPanelPrefab, alertsPanel);
        alertPanelInstance.name = document.Id;

        TextMeshProUGUI locationText = alertPanelInstance.transform.Find("Location").GetComponent<TextMeshProUGUI>();
        locationText.text = $"{alertData["location"].ToString()}";

        TextMeshProUGUI createdTimeText = alertPanelInstance.transform.Find("CreationTime").GetComponent<TextMeshProUGUI>();
        createdTimeText.text = $"{formattedTime}";

        TextMeshProUGUI temperatureText = alertPanelInstance.transform.Find("temperatureText").GetComponent<TextMeshProUGUI>();
        temperatureText.text = alertData.ContainsKey("temperature") ? alertData["temperature"].ToString() : "-";
        temperatureText.color = alertData.ContainsKey("temperature") ? Color.red : Color.black;
        temperatureText.fontSize = 20;

        TextMeshProUGUI flameDetectedText = alertPanelInstance.transform.Find("flameDetectedText").GetComponent<TextMeshProUGUI>();
        flameDetectedText.text = alertData.ContainsKey("flameDetected") ? alertData["flameDetected"].ToString() : "-";
        flameDetectedText.color = alertData.ContainsKey("flameDetected") ? Color.red : Color.black;
        flameDetectedText.fontSize = 20;

        TextMeshProUGUI lightLevelText = alertPanelInstance.transform.Find("lightLevelText").GetComponent<TextMeshProUGUI>();
        lightLevelText.text = alertData.ContainsKey("lightLevel") ? alertData["lightLevel"].ToString() : "-";
        lightLevelText.color = alertData.ContainsKey("lightLevel") ? Color.red : Color.black;
        lightLevelText.fontSize = 20;

        TextMeshProUGUI humanDetectedText = alertPanelInstance.transform.Find("humanDetectedText").GetComponent<TextMeshProUGUI>();
        humanDetectedText.text = alertData.ContainsKey("humanDetected") ? "감지" : "-";
        humanDetectedText.color = alertData.ContainsKey("humanDetected") ? Color.red : Color.black;
        humanDetectedText.fontSize = 20;

        //TextMeshProUGUI gasLevelText = alertPanelInstance.transform.Find("gasLevelText").GetComponent<TextMeshProUGUI>();
        //gasLevelText.text = alertData.ContainsKey("gasLevel") ? alertData["gasLevel"].ToString() : "-";
        //gasLevelText.color = alertData.ContainsKey("gasLevel") ? Color.red : Color.black;
        //gasLevelText.fontSize = 20;
        
        alertsUI.SetActive(true);
    }
 

    void RemoveAlert(string documentId)
    {
        Transform alertTransform = alertsPanel.Find(documentId);
        if (alertTransform != null)
        {
            Destroy(alertTransform.gameObject);
        }
    }
    public void OnCloseButtonClicked()
    {
        alertsUI.SetActive(false);
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true; 
    }
    public void OnActiveButtonClicked()
    {
        alertsUI.SetActive(true);
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = false;
    }
    public void OnSolvedButtonClicked(string documentId)
    {
        db.Collection("IssueAlerts").Document(documentId).DeleteAsync();
    }   
}
