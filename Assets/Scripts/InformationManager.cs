using UnityEngine;
using TMPro;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;

public class InformationManager : MonoBehaviour
{
    public GameObject informationPanel; 
    private FirebaseFirestore db;
    public string selectedMarkerId;
    public TMP_Text informationText; // ��Ŀ ������ ǥ���� �ؽ�Ʈ �ʵ�
    public TMP_Text levelText; // ��Ŀ ������ ǥ���� �ؽ�Ʈ �ʵ�
    public TMP_Text timestampText; // ���� �ð��� ǥ���� �ؽ�Ʈ �ʵ�
    public TMP_Text locationText;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        informationPanel.SetActive(false); // ���� �� Information Panel�� ��Ȱ��ȭ

    }

    public void OnEditButtonClicked()
    {

    }

    public void OnDeleteButtonClicked()
    {
        // Firestore���� ��Ŀ ������ ����
        db.Collection("markers").Document(selectedMarkerId).DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"Marker {selectedMarkerId} deleted successfully from Firestore.");

                // Unity ������ ��Ŀ GameObject ����
                GameObject markerGameObject = GameObject.Find(selectedMarkerId); // ������ Find ���, �����δ� �� ȿ������ ���� ��� ���
                if (markerGameObject != null)
                {
                    Destroy(markerGameObject);
                    Debug.Log($"Marker GameObject {selectedMarkerId} destroyed.");
                }
                else
                {
                    Debug.LogError("Failed to find Marker GameObject to destroy.");
                }

                informationPanel.SetActive(false); // ���� �г� ��Ȱ��ȭ
            }
            else
            {
                Debug.LogError("Error deleting marker from Firestore.");
            }
        });
    }

    public void SetSelectedMarkerId(string id)
    {
        selectedMarkerId = id;
    }

    public void OnCloseButtonClicked()
    {
        informationPanel.SetActive(false); // X ��ư Ŭ�� �� Information Panel ��Ȱ��ȭ
    }
    public void DisplayInformation(string information, int level, DateTime creationTime, string location)
    {
        informationText.text = $"Information: {information}";
        levelText.text = $"Level: {level}";
        timestampText.text = $"Created: {creationTime.ToString("yyyy-MM-dd HH:mm:ss")}";
        locationText.text = $"Location: {location}";

        // ���� �г� Ȱ��ȭ
        informationPanel.SetActive(true);
    }

   }
