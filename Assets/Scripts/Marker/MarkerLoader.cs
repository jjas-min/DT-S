using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MarkerLoader : MonoBehaviour
{
    private FirebaseFirestore db;
    public GameObject markerPrefab; // ��Ŀ ������
    public GameObject markerContainer; // ��Ŀ���� ���� �����̳� GameObject
    public InformationManager informationManager; // InformationManager ����
    public GameObject informationPanel; // ���� �г�

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
                    // ��Ŀ �����͸� ��ųʸ��� ��ȯ
                    Dictionary<string, object> markerData = document.ToDictionary();
                    // ��ųʸ����� ��ġ ������ ����
                    Dictionary<string, object> positionData = markerData["position"] as Dictionary<string, object>;
                    // ��ġ �����͸� ����Ͽ� Vector3 ��ü ����
                    Vector3 position = new Vector3(
                        Convert.ToSingle(positionData["x"]),
                        Convert.ToSingle(positionData["y"]),
                        Convert.ToSingle(positionData["z"])
                    );

                    // ����, ����, ���� �ð� ����
                    string information = markerData["information"] != null ? markerData["information"].ToString() : "";
                    int level = markerData.ContainsKey("level") ? int.Parse(markerData["level"].ToString()) : 1; // �⺻ ������ 0���� ����
                    DateTime creationTime;
                    if (markerData.ContainsKey("creationTime") && markerData["creationTime"] is Timestamp timestamp)
                    {
                        creationTime = timestamp.ToDateTime();
                    }
                    else
                    {
                        creationTime = DateTime.UtcNow; // �⺻������ ���� �ð��� ���
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

                        // �߰� ���� ����
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
