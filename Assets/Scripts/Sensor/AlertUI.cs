using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro;

public class SensorAlert : MonoBehaviour
{
    public GameObject alertsUI;
    public GameObject alertPanelPrefab; // 작은 패널의 프리팹
    public Transform alertsPanel; // ScrollView의 Content Transform
    private FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        ListenAlerts();
    }

    void ListenAlerts()
    {
        db.Collection("IssueAlerts").Listen(snapshot =>
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

        string formattedTime = TimeZoneInfo.ConvertTimeFromUtc(((Timestamp)alertData["createdTime"]).ToDateTime(), TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time")).ToString("yyyy-MM-dd HH:mm:ss");

        GameObject alertPanelInstance = Instantiate(alertPanelPrefab, alertsPanel);
        alertPanelInstance.name = document.Id; // Set the name of the instance to document ID for easy retrieval

        TextMeshProUGUI alertText = alertPanelInstance.GetComponentInChildren<TextMeshProUGUI>();
        alertText.text = $"Location: {alertData["location"].ToString()}_{alertData["sensorPackageNum"].ToString()}\nCreatedTime: {formattedTime}";

        foreach (var field in alertData)
        {
            if (field.Key != "createdTime" && field.Key != "location" && field.Key != "sensorPackageNum")
            {
                alertText.text += $"\n{field.Key}: {field.Value}";
            }
        }
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
    }
    public void OnActiveButtonClicked()
    {
        alertsUI.SetActive(true);
    }
}
