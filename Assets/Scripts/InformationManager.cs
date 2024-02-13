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
    public TMP_Text informationText; // 마커 정보를 표시할 텍스트 필드
    public TMP_Text levelText; // 마커 레벨을 표시할 텍스트 필드
    public TMP_Text timestampText; // 생성 시간을 표시할 텍스트 필드

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        informationPanel.SetActive(false); // 시작 시 Information Panel을 비활성화

    }

    public void OnEditButtonClicked()
    {

    }

    public void OnDeleteButtonClicked()
    {
        // Firestore에서 마커 데이터 삭제
        db.Collection("markers").Document(selectedMarkerId).DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"Marker {selectedMarkerId} deleted successfully from Firestore.");

                // Unity 씬에서 마커 GameObject 삭제
                GameObject markerGameObject = GameObject.Find(selectedMarkerId); // 예제로 Find 사용, 실제로는 더 효율적인 관리 방법 사용
                if (markerGameObject != null)
                {
                    Destroy(markerGameObject);
                    Debug.Log($"Marker GameObject {selectedMarkerId} destroyed.");
                }
                else
                {
                    Debug.LogError("Failed to find Marker GameObject to destroy.");
                }

                informationPanel.SetActive(false); // 정보 패널 비활성화
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
        informationPanel.SetActive(false); // X 버튼 클릭 시 Information Panel 비활성화
    }

   /* public void DisplayInformation()
    {
        var docRef = db.Collection("markers").Document(selectedMarkerId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                var snapshot = task.Result;
                if (snapshot.Exists)
                {
                    Dictionary<string, object> markerData = snapshot.ToDictionary();
                    string information = markerData.ContainsKey("information") ? markerData["information"].ToString() : "N/A";
                    int level = markerData.ContainsKey("level") ? int.Parse(markerData["level"].ToString()) : 0;
                    Timestamp creationTime = markerData.ContainsKey("creationTime") ? (Timestamp)markerData["creationTime"] : Timestamp.GetCurrentTimestamp();

                    informationText.text = $"Information: {information}";
                    levelText.text = $"Level: {level}";
                    timestampText.text = $"Created: {creationTime.ToDateTime():yyyy-MM-dd HH:mm:ss}";
                }
            }
        });
    }*/
}
