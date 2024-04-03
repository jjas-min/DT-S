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
    // Camera
    public GameObject firstPersonView;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        selectionUI.SetActive(false);
        MarkerLoader();
    }


    void Update()
    {
        positionSelector();

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

    void positionSelector() //���콺 ��Ŭ�� �� �ش� ������ ����Ʈ�� �������� ��Ŀ ���� UI ǥ��
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

    void ShowSelectionUI(Vector3 hitPoint, RaycastHit hit) //UI ǥ�� �� ���� ����
    {
        selectionUI.SetActive(true);
        // 카메라 이동 off
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = false;

        confirmButton.onClick.RemoveAllListeners(); 
        confirmButton.onClick.AddListener(() => {
            string information = informationInputField.text;
            int levelIndex = levelInputField.value;
            MarkerCreator(hitPoint, information, levelIndex, hit);
            informationInputField.text = "";
            levelInputField.value = 0; 
            selectionUI.SetActive(false);
            firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
        });
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() => {
            informationInputField.text = "";
            levelInputField.value = 0;
            selectionUI.SetActive(false);
            firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
        });
    }

    void MarkerCreator(Vector3 hitPoint, string information, int levelIndex, RaycastHit hit) //������ ��Ŀ ������ DB�� ����
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
            {"location", location },
            {"isSolved", false }
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
                switch (change.ChangeType) // ���⸦ ����
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
        bool isSolved = (bool)markerData["isSolved"];
        // ��Ŀ�� �̹� �����ϴ��� Ȯ��
        GameObject existingMarker = GameObject.Find(document.Id);
        if (existingMarker != null)
        {
            // ��Ŀ�� �̹� �����ϸ� ��ġ�� ��Ÿ ������ ������Ʈ
            existingMarker.transform.position = position;
            // ��Ÿ ������ ������Ʈ ���� �߰�
            UpdateMarkerData(existingMarker, markerData);
        }
        else
        {
            // �� ��Ŀ ����
            GameObject markerInstance = Instantiate(markerPrefab, position, Quaternion.identity, markerContainer.transform);
            markerInstance.name = document.Id; // GameObject�� �̸��� ���� ID�� ����
                                               // ��Ŀ ������ ����
            MarkerData markerComponent = markerInstance.AddComponent<MarkerData>();
            markerComponent.id = document.Id;
            markerComponent.position = position;
            markerComponent.information = information;
            markerComponent.level = level;
            markerComponent.creationTime = creationTime.ToDateTime();
            markerComponent.location = location;
            markerComponent.isSolved = isSolved;

            MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
            MarkerInfoManager infoManager = FindObjectOfType<MarkerInfoManager>();
            clickDetector.markerInfoManager = infoManager;

            if (markerComponent.isSolved) markerInstance.SetActive(false);
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
            // ���� ������Ʈ�� ������ ������Ʈ
            markerComponent.information = markerData["information"].ToString();
            markerComponent.level = Convert.ToInt32(markerData["level"]);
            markerComponent.creationTime = ((Timestamp)markerData["creationTime"]).ToDateTime();
            markerComponent.location = markerData["location"].ToString();
            markerComponent.isSolved = (bool)markerData["isSolved"];

            if (markerComponent.isSolved) marker.SetActive(false);
            // ��Ÿ �ʿ��� ������ ������Ʈ
        }
    }
}
