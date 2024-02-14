using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro ���ӽ����̽� �߰�


public class MarkerPlacer : MonoBehaviour
{
    public GameObject markerPrefab; // ǥ�� ������
    public GameObject selectionUI; // ���� UI �г�
    private Camera currentCamera; // ���� ī�޶� ������Ʈ
    public Button confirmButton; // "Ȯ��" ��ư
    public Button cancelButton; // "���" ��ư
    private Vector3 positionToPlaceMarker;
    public GameObject informationPanel; // ���� �г�
    private FirebaseFirestore db;
    public InformationManager informationManager; // InformationManager ���� �߰�
    public TMP_InputField informationInputField; // Information �Է� �ʵ�
    public TMP_Dropdown levelInputField; // Level �Է� �ʵ� �Ǵ� Dropdown ������Ʈ
    public GameObject markerContainer; // MarkerInstance GameObject�� ���� ����


    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        currentCamera = GetComponent<Camera>(); // ���� ������Ʈ�� ������ ī�޶� ������Ʈ�� �����ɴϴ�.
        selectionUI.SetActive(false); // ���� �� UI �г��� ��Ȱ��ȭ�մϴ�.
                                      // "Ȯ��" ��ư �̺�Ʈ


        // "���" ��ư �̺�Ʈ
        cancelButton.onClick.AddListener(() => {
            ResetInputFields(); // �Է� �ʵ�� ��Ӵٿ� �ʱ�ȭ
            selectionUI.SetActive(false); // �г��� ��Ȱ��ȭ�մϴ�.
        });
        // Firebase �ʱ�ȭ �� Firestore �ν��Ͻ� ��������
        db = FirebaseFirestore.DefaultInstance;
        LoadMarkersFromFirestore(); // ���� ���� �� Firestore�κ��� ��Ŀ �ε�

    }

    void Update()
    {
        if (!currentCamera.enabled) return;
        // ���콺 ��Ŭ���� �����մϴ�.
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition); // ���� ī�޶󿡼� ���콺 ��ġ�� Ray�� �����մϴ�.
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Raycast�� ������Ʈ�� ������ �ش� ��ġ�� ǥ���� ���� �� �ִ��� ����ڿ��� �����ϴ�.
                ShowSelectionUI(hit.point);
            }
        }
    }
    void ResetInputFields()
    {
        informationInputField.text = ""; // ���� �Է� �ʵ� �ʱ�ȭ
        levelInputField.value = 0; // ���� ��Ӵٿ� �ʱ�ȭ (0�� ù ��° �ɼ�)
    }
    void ShowSelectionUI(Vector3 hitPoint)
    {
        selectionUI.SetActive(true); // ����� ���� UI�� Ȱ��ȭ�մϴ�.
        Button confirmButton = selectionUI.GetComponentInChildren<Button>();
        confirmButton.onClick.RemoveAllListeners(); // ������ ��� �����ʸ� �����մϴ�.
        confirmButton.onClick.AddListener(() => {
            string information = informationInputField.text;
            int levelIndex = levelInputField.value; 
            PlaceMarker(hitPoint, information, levelIndex);
            ResetInputFields();
            selectionUI.SetActive(false);
        }); // ���ο� �����ʸ� �߰��մϴ�.
    }

    void PlaceMarker(Vector3 hitPoint, string information, int levelIndex)
    {
        Vector3 placePosition = hitPoint;

        GameObject markerInstance = Instantiate(markerPrefab, placePosition, Quaternion.identity, markerContainer.transform); // ǥ���� �����մϴ�.
        
        MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
        clickDetector.informationPanel = informationPanel;
        clickDetector.informationManager = this.informationManager;

        int actualLevel = levelIndex + 1;

        MarkerData markerData = new MarkerData(placePosition, information, actualLevel);
        clickDetector.markerId = markerData.id;

        Dictionary<string, object> markerDict = new Dictionary<string, object>
        {
            { "id", markerData.id },
            { "position", new Dictionary<string, object> {
                { "x", placePosition.x },
                { "y", placePosition.y },
                { "z", placePosition.z }
            }},
            {"information", markerData.information},
            {"level", actualLevel},
            {"creationTime", Timestamp.FromDateTime(DateTime.UtcNow)} // DateTime ��ü ���� ���
        };
        markerInstance.name = markerData.id;
        db.Collection("markers").Document(markerData.id).SetAsync(markerDict);
        selectionUI.SetActive(false); // ǥ�� ���� �� UI �г��� ��Ȱ��ȭ�մϴ�.
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
                    GameObject markerInstance = Instantiate(markerPrefab, position, Quaternion.identity, markerContainer.transform);
                    markerInstance.name = document.Id;
                    MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
                    if (clickDetector != null)
                    {
                        clickDetector.informationPanel = informationPanel;
                        clickDetector.markerId = document.Id;
                        clickDetector.informationManager = this.informationManager;

                        // �߰� ���� ����
                        clickDetector.SetMarkerDetails(information, level, creationTime);
                    }
                }
            }
            else
            {
                Debug.LogError("Failed to load marker data from Firestore.");
            }
        });
    }



}
