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
    private FirebaseFirestore db;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = false;
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

        // 이미지 생성 후 Vertical Layout Group 컴포넌트 추가 및 설정
        VerticalLayoutGroup layoutGroup = alertsPanel.gameObject.AddComponent<VerticalLayoutGroup>();
        layoutGroup.spacing = 10f;
        
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
