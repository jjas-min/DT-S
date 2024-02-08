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
    void Start()
    {
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
        // 카메라에서 히트 포인트 방향으로 조금 더 가까운 위치에 표식을 배치합니다.
        Vector3 directionToCamera = (currentCamera.transform.position - hitPoint).normalized; // 카메라 방향의 단위 벡터
        float offsetDistance = 0.1f; // 카메라 방향으로 얼마나 뒤로 물러날지에 대한 거리
        Vector3 placePosition = hitPoint + directionToCamera * offsetDistance; // 실제 표식을 배치할 위치

        GameObject markerInstance = Instantiate(markerPrefab, placePosition, Quaternion.identity); // 표식을 생성합니다.
        MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
        clickDetector.informationPanel = informationPanel;
        selectionUI.SetActive(false); // 표식 생성 후 UI 패널을 비활성화합니다.
    }

}
