using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        currentCamera = GetComponent<Camera>(); // ���� ������Ʈ�� ������ ī�޶� ������Ʈ�� �����ɴϴ�.
        selectionUI.SetActive(false); // ���� �� UI �г��� ��Ȱ��ȭ�մϴ�.
                                      // "Ȯ��" ��ư �̺�Ʈ
        confirmButton.onClick.AddListener(() => {
            PlaceMarker(positionToPlaceMarker);
            selectionUI.SetActive(false); // �г��� ��Ȱ��ȭ�մϴ�.
        });

        // "���" ��ư �̺�Ʈ
        cancelButton.onClick.AddListener(() => {
            selectionUI.SetActive(false); // �г��� ��Ȱ��ȭ�մϴ�.
        });
        // Firebase �ʱ�ȭ �� Firestore �ν��Ͻ� ��������
        db = FirebaseFirestore.DefaultInstance;
        LoadMarkersFromFirestore(); // ���� ���� �� Firestore�κ��� ��Ŀ �ε�

    }

    void Update()
    {
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

    void ShowSelectionUI(Vector3 hitPoint)
    {
        selectionUI.SetActive(true); // ����� ���� UI�� Ȱ��ȭ�մϴ�.
        Button confirmButton = selectionUI.GetComponentInChildren<Button>();
        confirmButton.onClick.RemoveAllListeners(); // ������ ��� �����ʸ� �����մϴ�.
        confirmButton.onClick.AddListener(() => PlaceMarker(hitPoint)); // ���ο� �����ʸ� �߰��մϴ�.
    }

    void PlaceMarker(Vector3 hitPoint)
    {
        Vector3 placePosition = hitPoint;

        GameObject markerInstance = Instantiate(markerPrefab, placePosition, Quaternion.identity); // ǥ���� �����մϴ�.
        MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
        clickDetector.informationPanel = informationPanel;
        clickDetector.informationManager = this.informationManager;
        MarkerData markerData = new MarkerData(placePosition, ""); // ������ ���߿� �߰�
        clickDetector.markerId = markerData.id;
        Dictionary<string, object> markerDict = new Dictionary<string, object>
        {
            { "id", markerData.id },
            { "position", new Dictionary<string, object> {
                { "x", placePosition.x },
                { "y", placePosition.y },
                { "z", placePosition.z }
            }},
            { "information", markerData.information }
        };
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
                    string information = markerData["information"] != null ? markerData["information"].ToString() : "";
                    // ��Ŀ �������� �ν��Ͻ�ȭ�ϰ� ��ġ ����
                    GameObject markerInstance = Instantiate(markerPrefab, position, Quaternion.identity);

                    // �ʿ��� ���, ��Ŀ �ν��Ͻ��� �߰� ���� ����
                    // ��: ��Ŀ �ν��Ͻ��� MarkerClickDetector ������Ʈ�� �߰��Ǿ� ������ ���� �г� ���� ����
                    MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
                    if (clickDetector != null)
                    {
                        clickDetector.informationPanel = informationPanel;
                        clickDetector.markerId = document.Id; // ��Ŀ ID ����
                        clickDetector.informationManager = this.informationManager;
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
