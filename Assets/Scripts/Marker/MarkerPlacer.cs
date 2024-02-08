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
    public GameObject markerPrefab; // 표식 프리팹
    public GameObject selectionUI; // 선택 UI 패널
    private Camera currentCamera; // 현재 카메라 컴포넌트
    public Button confirmButton; // "확인" 버튼
    public Button cancelButton; // "취소" 버튼
    private Vector3 positionToPlaceMarker;
    public GameObject informationPanel; // 정보 패널
    private FirebaseFirestore db;
    public InformationManager informationManager; // InformationManager 참조 추가

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        currentCamera = GetComponent<Camera>(); // 현재 오브젝트에 부착된 카메라 컴포넌트를 가져옵니다.
        selectionUI.SetActive(false); // 시작 시 UI 패널을 비활성화합니다.
                                      // "확인" 버튼 이벤트
        confirmButton.onClick.AddListener(() => {
            PlaceMarker(positionToPlaceMarker);
            selectionUI.SetActive(false); // 패널을 비활성화합니다.
        });

        // "취소" 버튼 이벤트
        cancelButton.onClick.AddListener(() => {
            selectionUI.SetActive(false); // 패널을 비활성화합니다.
        });
        // Firebase 초기화 및 Firestore 인스턴스 가져오기
        db = FirebaseFirestore.DefaultInstance;
        LoadMarkersFromFirestore(); // 게임 시작 시 Firestore로부터 마커 로드

    }

    void Update()
    {
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

    void ShowSelectionUI(Vector3 hitPoint)
    {
        selectionUI.SetActive(true); // 사용자 선택 UI를 활성화합니다.
        Button confirmButton = selectionUI.GetComponentInChildren<Button>();
        confirmButton.onClick.RemoveAllListeners(); // 기존의 모든 리스너를 제거합니다.
        confirmButton.onClick.AddListener(() => PlaceMarker(hitPoint)); // 새로운 리스너를 추가합니다.
    }

    void PlaceMarker(Vector3 hitPoint)
    {
        Vector3 placePosition = hitPoint;

        GameObject markerInstance = Instantiate(markerPrefab, placePosition, Quaternion.identity); // 표식을 생성합니다.
        MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
        clickDetector.informationPanel = informationPanel;
        clickDetector.informationManager = this.informationManager;
        MarkerData markerData = new MarkerData(placePosition, ""); // 정보는 나중에 추가
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
                    string information = markerData["information"] != null ? markerData["information"].ToString() : "";
                    // 마커 프리팹을 인스턴스화하고 위치 설정
                    GameObject markerInstance = Instantiate(markerPrefab, position, Quaternion.identity);

                    // 필요한 경우, 마커 인스턴스에 추가 정보 설정
                    // 예: 마커 인스턴스에 MarkerClickDetector 컴포넌트가 추가되어 있으면 정보 패널 참조 설정
                    MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
                    if (clickDetector != null)
                    {
                        clickDetector.informationPanel = informationPanel;
                        clickDetector.markerId = document.Id; // 마커 ID 설정
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
