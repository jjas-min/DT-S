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

    void positionSelector() //ï¿½ï¿½ï¿½ì½º ï¿½ï¿½Å¬ï¿½ï¿½ ï¿½ï¿½ ï¿½Ø´ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½Æ®ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½Ä¿ ï¿½ï¿½ï¿½ï¿½ UI Ç¥ï¿½ï¿½
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

    void ShowSelectionUI(Vector3 hitPoint, RaycastHit hit) //UI Ç¥ï¿½ï¿½ ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
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

    void MarkerCreator(Vector3 hitPoint, string information, int levelIndex, RaycastHit hit) //ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ ï¿½ï¿½Ä¿ ï¿½ï¿½ï¿½ï¿½ï¿½ï¿½ DBï¿½ï¿½ ï¿½ï¿½ï¿½ï¿½
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
                switch (change.ChangeType) // ¿©±â¸¦ ¼öÁ¤
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
        // ¸¶Ä¿°¡ ÀÌ¹Ì Á¸ÀçÇÏ´ÂÁö È®ÀÎ
        GameObject existingMarker = GameObject.Find(document.Id);
        if (existingMarker != null)
        {
            // ¸¶Ä¿°¡ ÀÌ¹Ì Á¸ÀçÇÏ¸é À§Ä¡¿Í ±âÅ¸ µ¥ÀÌÅÍ ¾÷µ¥ÀÌÆ®
            existingMarker.transform.position = position;
            // ±âÅ¸ µ¥ÀÌÅÍ ¾÷µ¥ÀÌÆ® ·ÎÁ÷ Ãß°¡
            UpdateMarkerData(existingMarker, markerData);
        }
        else
        {
            // »õ ¸¶Ä¿ »ý¼º
            GameObject markerInstance = Instantiate(markerPrefab, position, Quaternion.identity, markerContainer.transform);
            markerInstance.name = document.Id; // GameObjectÀÇ ÀÌ¸§À» ¹®¼­ ID·Î ¼³Á¤
                                               // ¸¶Ä¿ µ¥ÀÌÅÍ ¼³Á¤
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
            // ±âÁ¸ ÄÄÆ÷³ÍÆ®ÀÇ µ¥ÀÌÅÍ ¾÷µ¥ÀÌÆ®
            markerComponent.information = markerData["information"].ToString();
            markerComponent.level = Convert.ToInt32(markerData["level"]);
            markerComponent.creationTime = ((Timestamp)markerData["creationTime"]).ToDateTime();
            markerComponent.location = markerData["location"].ToString();
            // ±âÅ¸ ÇÊ¿äÇÑ µ¥ÀÌÅÍ ¾÷µ¥ÀÌÆ®
        }
    }
}
