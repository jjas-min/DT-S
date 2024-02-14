using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 네임스페이스 추가


public class MarkerPlacer : MonoBehaviour
{
    public GameObject markerPrefab; // 표식 프리팹
    public GameObject selectionUI; // 선택 UI 패널
    private Camera currentCamera; // 현재 카메라 컴포넌트
    public Button confirmButton; // "확인" 버튼
    public Button cancelButton; // "취소" 버튼
    private Vector3 positionToPlaceMarker;
    public GameObject informationPanel; // 정보 패널
    private FirebaseFirestore db;
    public InformationManager informationManager; // InformationManager 참조 추가
    public TMP_InputField informationInputField; // Information 입력 필드
    public TMP_Dropdown levelInputField; // Level 입력 필드 또는 Dropdown 컴포넌트
    public GameObject markerContainer; // MarkerInstance GameObject에 대한 참조


    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        currentCamera = GetComponent<Camera>(); // 현재 오브젝트에 부착된 카메라 컴포넌트를 가져옵니다.
        selectionUI.SetActive(false); // 시작 시 UI 패널을 비활성화합니다.
                                      // "확인" 버튼 이벤트


        // "취소" 버튼 이벤트
        cancelButton.onClick.AddListener(() => {
            ResetInputFields(); // 입력 필드와 드롭다운 초기화
            selectionUI.SetActive(false); // 패널을 비활성화합니다.
        });
        // Firebase 초기화 및 Firestore 인스턴스 가져오기
        db = FirebaseFirestore.DefaultInstance;
        LoadMarkersFromFirestore(); // 게임 시작 시 Firestore로부터 마커 로드

    }

    void Update()
    {
        if (!currentCamera.enabled) return;
        // 마우스 우클릭을 감지합니다.
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition); // 현재 카메라에서 마우스 위치로 Ray를 생성합니다.
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Raycast가 오브젝트에 닿으면 해당 위치에 표식을 남길 수 있는지 사용자에게 묻습니다.
                ShowSelectionUI(hit.point);
            }
        }
    }
    void ResetInputFields()
    {
        informationInputField.text = ""; // 정보 입력 필드 초기화
        levelInputField.value = 0; // 레벨 드롭다운 초기화 (0이 첫 번째 옵션)
    }
    void ShowSelectionUI(Vector3 hitPoint)
    {
        selectionUI.SetActive(true); // 사용자 선택 UI를 활성화합니다.
        Button confirmButton = selectionUI.GetComponentInChildren<Button>();
        confirmButton.onClick.RemoveAllListeners(); // 기존의 모든 리스너를 제거합니다.
        confirmButton.onClick.AddListener(() => {
            string information = informationInputField.text;
            int levelIndex = levelInputField.value; 
            PlaceMarker(hitPoint, information, levelIndex);
            ResetInputFields();
            selectionUI.SetActive(false);
        }); // 새로운 리스너를 추가합니다.
    }

    void PlaceMarker(Vector3 hitPoint, string information, int levelIndex)
    {
        Vector3 placePosition = hitPoint;

        GameObject markerInstance = Instantiate(markerPrefab, placePosition, Quaternion.identity, markerContainer.transform); // 표식을 생성합니다.
        
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
            {"creationTime", Timestamp.FromDateTime(DateTime.UtcNow)} // DateTime 객체 직접 사용
        };
        markerInstance.name = markerData.id;
        db.Collection("markers").Document(markerData.id).SetAsync(markerDict);
        selectionUI.SetActive(false); // 표식 생성 후 UI 패널을 비활성화합니다.
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
                    GameObject markerInstance = Instantiate(markerPrefab, position, Quaternion.identity, markerContainer.transform);
                    markerInstance.name = document.Id;
                    MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
                    if (clickDetector != null)
                    {
                        clickDetector.informationPanel = informationPanel;
                        clickDetector.markerId = document.Id;
                        clickDetector.informationManager = this.informationManager;

                        // 추가 정보 설정
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
