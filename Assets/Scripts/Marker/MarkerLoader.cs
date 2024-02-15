using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MarkerLoader : MonoBehaviour
{
    private FirebaseFirestore db;
    public GameObject markerPrefab; // 마커 프리팹
    public GameObject markerContainer; // 마커들을 담을 컨테이너 GameObject
    public InformationManager informationManager; // InformationManager 참조
    public GameObject informationPanel; // 정보 패널

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        LoadMarkersFromFirestore();
    }

    void LoadMarkersFromFirestore()
    {
        db.Collection("markers").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                foreach (DocumentSnapshot document in task.Result.Documents)
                {
                    // 마커 데이터를 딕셔너리로 변환
                    Dictionary<string, object> markerData = document.ToDictionary();
                    // 딕셔너리에서 위치 데이터 추출
                    Dictionary<string, object> positionData = markerData["position"] as Dictionary<string, object>;
                    // 위치 데이터를 사용하여 Vector3 객체 생성
                    Vector3 position = new Vector3(
                        Convert.ToSingle(positionData["x"]),
                        Convert.ToSingle(positionData["y"]),
                        Convert.ToSingle(positionData["z"])
                    );

                    // 정보, 레벨, 생성 시간 추출
                    string information = markerData["information"] != null ? markerData["information"].ToString() : "";
                    int level = markerData.ContainsKey("level") ? int.Parse(markerData["level"].ToString()) : 1; // 기본 레벨을 0으로 가정
                    DateTime creationTime;
                    if (markerData.ContainsKey("creationTime") && markerData["creationTime"] is Timestamp timestamp)
                    {
                        creationTime = timestamp.ToDateTime();
                    }
                    else
                    {
                        creationTime = DateTime.UtcNow; // 기본값으로 현재 시간을 사용
                    }
                    string location = markerData["location"] != null ? markerData["location"].ToString() : "";
                    GameObject markerInstance = Instantiate(markerPrefab, position, Quaternion.identity, markerContainer.transform);
                    markerInstance.name = document.Id;
                    MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
                    if (clickDetector != null)
                    {
                        clickDetector.informationPanel = informationPanel;
                        clickDetector.markerId = document.Id;
                        clickDetector.informationManager = this.informationManager;

                        // 추가 정보 설정
                        clickDetector.SetMarkerDetails(information, level, creationTime, location);
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to load marker data from Firestore: " + task.Exception.ToString());
            }
        });
    }
}
