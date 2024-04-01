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
    public GameObject alertPanelPrefab; // ���� �г��� ������
    public GameObject firstPersonView;
    public Transform alertsPanel; // ScrollView�� Content Transform
    private TMP_Text temperatureText; 
    private TMP_Text lightLevelText;
    private TMP_Text flameDetectedText;
    private TMP_Text humanDetectedText;
    private TMP_Text locationText;
    private TMP_Text creationTimeText;
    private FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = false;
        
        // Find the TextMeshPro components among the children of the alertPanelPrefab
        temperatureText = alertPanelPrefab.transform.Find("Temperature").GetComponent<TMP_Text>();
        lightLevelText = alertPanelPrefab.transform.Find("LightLevel").GetComponent<TMP_Text>();
        flameDetectedText = alertPanelPrefab.transform.Find("FlameDetected").GetComponent<TMP_Text>();
        humanDetectedText = alertPanelPrefab.transform.Find("HumanDetected").GetComponent<TMP_Text>();
        locationText = alertPanelPrefab.transform.Find("location").GetComponent<TMP_Text>();
        creationTimeText = alertPanelPrefab.transform.Find("CreationTime").GetComponent<TMP_Text>();

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

        GameObject alertPanelInstance = Instantiate(alertPanelPrefab, alertsUI.transform);

        // Update sensor information on the alert panel
        temperatureText.text = alertData.ContainsKey("temperature") ? $"<color=red>{alertData["temperature"]}</color>" : "-";
        lightLevelText.text = alertData.ContainsKey("lightLevel") ? $"<color=red>{alertData["lightLevel"]}</color>" : "-";
        flameDetectedText.text = alertData.ContainsKey("flameDetected") ? $"<color=red>{alertData["flameDetected"]}</color>" : "-";
        humanDetectedText.text = alertData.ContainsKey("humanDetected") ? $"<color=red>{alertData["humanDetected"]}</color>" : "-";
        locationText.text = alertData.ContainsKey("location") ? $"{alertData["location"]}</color>" : "-";
        string formattedTime = TimeZoneInfo.ConvertTimeFromUtc(((Timestamp)alertData["createdTime"]).ToDateTime(), TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time")).ToString("yyyy.MM.dd hh:mm tt");
        creationTimeText.text = $"{formattedTime}";

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
}
