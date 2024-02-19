using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using TMPro;

public class MarkerManager : MonoBehaviour
{
    private FirebaseFirestore db;
    //UI
    public GameObject selectionUI;
    public Button confirmButton; 
    public Button cancelButton;
    public TMP_InputField informationInputField;
    public TMP_Dropdown levelInputField;
    //Marker
    public GameObject markerContainer;
    public GameObject markerPrefab;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        selectionUI.SetActive(false);
        MarkerLoader();
    }


    void Update()
    {
        positionSelector();
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => {
            informationInputField.text = "";
            levelInputField.value = 0;
            selectionUI.SetActive(false);
        });
    }

    Camera FindActiveCamera()
    {
        foreach (Camera cam in Camera.allCameras)
        {
            if (cam.isActiveAndEnabled)
            {
                return cam;
            }
        }
        return null;
    }

    void positionSelector() //마우스 우클릭 시 해당 지점의 포인트를 기준으로 마커 생성 UI 표시
    {
        Camera activeCamera = FindActiveCamera();
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = activeCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                ShowSelectionUI(hit.point, hit);
            }
        }
    }

    void ShowSelectionUI(Vector3 hitPoint, RaycastHit hit) //UI 표시 및 정보 전달
    {
        selectionUI.SetActive(true); 
        Button confirmButton = selectionUI.GetComponentInChildren<Button>();
        Button cancelButton = selectionUI.GetComponentInChildren<Button>();
        confirmButton.onClick.RemoveAllListeners(); 
        confirmButton.onClick.AddListener(() => {
            string information = informationInputField.text;
            int levelIndex = levelInputField.value;
            MarkerCreator(hitPoint, information, levelIndex, hit);
            informationInputField.text = "";
            levelInputField.value = 0; 
            selectionUI.SetActive(false);
        });
    }

    void MarkerCreator(Vector3 hitPoint, string information, int levelIndex, RaycastHit hit) //생성된 마커 정보를 DB에 저장
    {
        Vector3 placePosition = hitPoint;
        string location = hit.collider.gameObject.name;
        int actualLevel = levelIndex + 1;
        string markerId = System.Guid.NewGuid().ToString();

        Dictionary<string, object> markerDict = new Dictionary<string, object>
        {
            { "id", markerId},
            { "position", new Dictionary<string, object> {
                { "x", placePosition.x },
                { "y", placePosition.y },
                { "z", placePosition.z }
            }},
            {"information", information},
            {"level", actualLevel},
            {"creationTime", Timestamp.FromDateTime(DateTime.UtcNow)},
            {"location", location }
        };

        db.Collection("Markers").Document(markerId).SetAsync(markerDict);
        selectionUI.SetActive(false);
    }
    void MarkerLoader()
    {
        FirebaseFirestore db = FirebaseFirestore.DefaultInstance;
        db.Collection("Markers").Listen(snapshot =>
        {
            foreach (var change in snapshot.GetChanges())
            {
                switch (change.ChangeType) // 여기를 수정
                {
                    case DocumentChange.Type.Added:
                        Debug.Log($"New document added: {change.Document.Id}");
                        CreateOrUpdateMarker(change.Document);
                        break;
                    case DocumentChange.Type.Modified:
                        Debug.Log($"Document modified: {change.Document.Id}");
                        CreateOrUpdateMarker(change.Document);
                        break;
                    case DocumentChange.Type.Removed:
                        Debug.Log($"Document removed: {change.Document.Id}");
                        RemoveMarker(change.Document.Id);
                        break;
                }
            }
        });
    }


    void CreateOrUpdateMarker(DocumentSnapshot document)
    {
        Dictionary<string, object> markerData = document.ToDictionary();
        Dictionary<string, object> positionData = markerData["position"] as Dictionary<string, object>;
        Vector3 position = new Vector3(
            Convert.ToSingle(positionData["x"]),
            Convert.ToSingle(positionData["y"]),
            Convert.ToSingle(positionData["z"])
        );
        string information = markerData["information"].ToString();
        int level = int.Parse(markerData["level"].ToString());
        Timestamp creationTime = (Timestamp)markerData["creationTime"];
        string location = markerData["location"].ToString();
        // 마커가 이미 존재하는지 확인
        GameObject existingMarker = GameObject.Find(document.Id);
        if (existingMarker != null)
        {
            // 마커가 이미 존재하면 위치와 기타 데이터 업데이트
            existingMarker.transform.position = position;
            // 기타 데이터 업데이트 로직 추가
            UpdateMarkerData(existingMarker, markerData);
        }
        else
        {
            // 새 마커 생성
            GameObject markerInstance = Instantiate(markerPrefab, position, Quaternion.identity, markerContainer.transform);
            markerInstance.name = document.Id; // GameObject의 이름을 문서 ID로 설정
                                               // 마커 데이터 설정
            MarkerData markerComponent = markerInstance.AddComponent<MarkerData>();
            markerComponent.id = document.Id;
            markerComponent.position = position;
            markerComponent.information = information;
            markerComponent.level = level;
            markerComponent.creationTime = creationTime.ToDateTime();
            markerComponent.location = location;

            MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
            MarkerInfoManager infoManager = FindObjectOfType<MarkerInfoManager>();
            clickDetector.markerInfoManager = infoManager;
        }
    }

    void RemoveMarker(string documentId)
    {
        GameObject markerToBeRemoved = GameObject.Find(documentId);
        if (markerToBeRemoved != null)
        {
            Destroy(markerToBeRemoved);
            Debug.Log($"Marker {documentId} removed successfully.");
        }
        else
        {
            Debug.LogWarning($"Marker {documentId} not found for removal.");
        }
    }
   

    void UpdateMarkerData(GameObject marker, Dictionary<string, object> markerData)
    {
        MarkerData markerComponent = marker.GetComponent<MarkerData>();
        if (markerComponent != null)
        {
            // 기존 컴포넌트의 데이터 업데이트
            markerComponent.information = markerData["information"].ToString();
            markerComponent.level = Convert.ToInt32(markerData["level"]);
            markerComponent.creationTime = ((Timestamp)markerData["creationTime"]).ToDateTime();
            markerComponent.location = markerData["location"].ToString();
            // 기타 필요한 데이터 업데이트
        }
    }
}
