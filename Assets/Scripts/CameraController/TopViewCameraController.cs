using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TopViewCameraController : MonoBehaviour
{
    public float dragSpeed = 30; // 드래그 속도
    public float zoomSpeed = 12; // 줌 속도
    public float minZoomDistance = 5.0f; // 최소 줌 거리
    public float maxZoomDistance = 100.0f; // 최대 줌 거리

    public GameObject viewSwitcher;
    private Vector3 dragOrigin; // 드래그 시작점
    private Camera currentCamera; // 현재 카메라에 대한 참조
    public Camera firstPersonViewCamera;
    public TMP_Dropdown roomsDropdown;

    void Start()
    {
        currentCamera = GetComponent<Camera>(); // 현재 오브젝트에 부착된 카메라 컴포넌트를 가져옵니다.
        roomsDropdown.value = 0; // Hallway 선택

        // 드랍다운 값 변경 이벤트에 MoveToRoom 함수 연결
        roomsDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(roomsDropdown);
        });
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        // 선택된 방 이름 가져오기
        string selectedRoom = change.options[change.value].text;
        // 선택된 방으로 이동
        viewSwitcher.SwitchToFirstPersonView();
        firstPersonViewCamera.MoveToRoom(selectedRoom);
    }

    void Update()
    {
        if (!currentCamera.enabled) return;
        // 마우스 스크롤을 통한 줌 인/아웃
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Vector3 pos = transform.position;
        pos.y -= scroll * zoomSpeed * 100f * Time.deltaTime;
        pos.y = Mathf.Clamp(pos.y, minZoomDistance, maxZoomDistance);
        transform.position = pos;

        // 마우스 드래그를 통한 카메라 이동
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            if (currentCamera == null) return; // 현재 카메라가 없으면 함수를 종료
            Vector3 posMove = currentCamera.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
            Vector3 move = new Vector3(-posMove.x * dragSpeed, 0, -posMove.y * dragSpeed);

            transform.Translate(move, Space.World);
            dragOrigin = Input.mousePosition;
        }
    }
}
