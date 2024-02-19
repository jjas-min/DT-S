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

        db.Collection("markers").Document(markerId).SetAsync(markerDict);
        selectionUI.SetActive(false);
    }
    void MarkerLoader()
    {
        db.Collection("markers").GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log("Marker data loaded successfully.");
                foreach (DocumentSnapshot document in task.Result.Documents)
                {
                    Dictionary<string, object> markerData = document.ToDictionary();
                    Dictionary<string, object> positionData = markerData["position"] as Dictionary<string, object>;
                    Vector3 position = new Vector3(
                        Convert.ToSingle(positionData["x"]),
                        Convert.ToSingle(positionData["y"]),
                        Convert.ToSingle(positionData["z"])
                    );
                    GameObject markerInstance = Instantiate(markerPrefab, position, Quaternion.identity, markerContainer.transform);
                    markerInstance.name = document.Id;

                    string information = markerData["information"].ToString();
                    int level = int.Parse(markerData["level"].ToString());
                    Timestamp creationTime = (Timestamp)markerData["creationTime"];
                    string location = markerData["location"].ToString();

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
        });
    }

}
