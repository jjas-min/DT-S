using UnityEngine;
using TMPro;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;

public class MarkerInfoManager : MonoBehaviour
{
    public GameObject informationPanel;
    private FirebaseFirestore db;

    public TMP_Text informationText; // ��Ŀ ������ ǥ���� �ؽ�Ʈ �ʵ�
    public TMP_Text levelText; // ��Ŀ ������ ǥ���� �ؽ�Ʈ �ʵ�
    public TMP_Text timestampText; // ���� �ð��� ǥ���� �ؽ�Ʈ �ʵ�
    public TMP_Text locationText;

    private MarkerData markerData;

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
        string id = markerData.id;

        db.Collection("markers").Document(id).DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"Marker {id} deleted successfully from Firestore.");


                GameObject markerGameObject = GameObject.Find(id);
                if (markerGameObject != null)
                {
                    Destroy(markerGameObject);
                    Debug.Log($"Marker GameObject {id} destroyed.");
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


    public void OnCloseButtonClicked()
    {
        informationPanel.SetActive(false); // X ��ư Ŭ�� �� Information Panel ��Ȱ��ȭ
    }

    public void DisplayInformation(MarkerData markerData)
    {
        this.markerData = markerData;
        informationText.text = $"Information: {markerData.information}";
        levelText.text = $"Level: {markerData.level}";
        timestampText.text = $"Created: {markerData.creationTime.ToString("yyyy-MM-dd HH:mm:ss")}";
        locationText.text = $"Location: {markerData.location}";

        informationPanel.SetActive(true);
    }
   }
