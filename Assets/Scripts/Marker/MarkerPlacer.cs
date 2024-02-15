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
                ShowSelectionUI(hit.point, hit);
            }
        }
    }
    void ResetInputFields()
    {
        informationInputField.text = ""; // ���� �Է� �ʵ� �ʱ�ȭ
        levelInputField.value = 0; // ���� ��Ӵٿ� �ʱ�ȭ (0�� ù ��° �ɼ�)
    }
    void ShowSelectionUI(Vector3 hitPoint, RaycastHit hit)
    {
        selectionUI.SetActive(true); // ����� ���� UI�� Ȱ��ȭ�մϴ�.
        Button confirmButton = selectionUI.GetComponentInChildren<Button>();
        confirmButton.onClick.RemoveAllListeners(); // ������ ��� �����ʸ� �����մϴ�.
        confirmButton.onClick.AddListener(() => {
            string information = informationInputField.text;
            int levelIndex = levelInputField.value; 
            PlaceMarker(hitPoint, information, levelIndex, hit);
            ResetInputFields();
            selectionUI.SetActive(false);
        }); // ���ο� �����ʸ� �߰��մϴ�.
    }

    void PlaceMarker(Vector3 hitPoint, string information, int levelIndex, RaycastHit hit)
    {
        Vector3 placePosition = hitPoint;
        string location = hit.collider.gameObject.name;
        GameObject markerInstance = Instantiate(markerPrefab, placePosition, Quaternion.identity, markerContainer.transform); // ǥ���� �����մϴ�.
        
        MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
        clickDetector.informationPanel = informationPanel;
        clickDetector.informationManager = this.informationManager;

        int actualLevel = levelIndex + 1;

        MarkerData markerData = new MarkerData(placePosition, information, actualLevel, location);
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
            {"creationTime", Timestamp.FromDateTime(DateTime.UtcNow)}, // DateTime ��ü ���� ���
            {"location", location }
        };
        markerInstance.name = markerData.id;
        db.Collection("markers").Document(markerData.id).SetAsync(markerDict);
        selectionUI.SetActive(false); // ǥ�� ���� �� UI �г��� ��Ȱ��ȭ�մϴ�.
    }
    



}
