using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FirstPersonViewCameraController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 플레이어 이동 속도

    public float rotateSpeed = 100.0f; // 회전 속도 조절 변수
    private Camera attachedCamera; // 현재 스크립트가 부착된 카메라
    private Vector3 prevMousePosition; // 이전 마우스 위치 저장 변수
    public TMP_Dropdown roomsDropdown;

    void Start()
    {
        // 현재 스크립트가 부착된 카메라 컴포넌트를 가져옴
        attachedCamera = GetComponent<Camera>();
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
        MoveToRoom(selectedRoom);
    }

    void Update()
    {
        // 카메라가 활성화되어 있지 않으면 처리를 하지 않습니다.
        if (!attachedCamera.enabled) return;

        // [회전]

        // 마우스 버튼(일반적으로 왼쪽 버튼)이 눌렸는지 확인
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 버튼이 처음 눌렸을 때, 현재 마우스 위치 저장
            prevMousePosition = Input.mousePosition;
        }

        // 마우스 버튼이 눌린 상태에서 움직이고 있는지 확인
        if (Input.GetMouseButton(0))
        {
            // 현재 마우스 위치와 이전 마우스 위치의 차이 계산
            Vector3 delta = Input.mousePosition - prevMousePosition;
            // Y축 회전은 마우스의 X축 움직임에, X축 회전은 마우스의 Y축 움직임에 반응하도록 설정
            float rotateY = delta.x * rotateSpeed * Time.deltaTime;
            float rotateX = -delta.y * rotateSpeed * Time.deltaTime;

            // 현재 오브젝트(카메라)의 회전에 마우스 움직임에 따른 회전값 적용
            transform.eulerAngles += new Vector3(rotateX, rotateY, 0);

            // 현재 마우스 위치를 이전 마우스 위치로 업데이트
            prevMousePosition = Input.mousePosition;
        }

        // [이동]

        float x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // A, D 키 혹은 좌우 화살표
        float z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime; // W, S 키 혹은 상하 화살표

        // 플레이어 이동
        transform.Translate(x, 0, z);

        // 'e' 키를 누를 때
        if (Input.GetKey(KeyCode.E))
        {
            MoveUp();
        }

        // 'q' 키를 누를 때
        if (Input.GetKey(KeyCode.Q))
        {
            MoveDown();
        }


    }

    void MoveUp()
    {
        // 현재 위치에서 y좌표를 올림
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
    }

    void MoveDown()
    {
        // 현재 위치에서 y좌표를 내림
        transform.position -= new Vector3(0, moveSpeed * Time.deltaTime, 0);
    }

    void MoveToRoom(string roomName)
    {
        Vector3 targetPosition = Vector3.zero;
        Vector3 targetRotation = Vector3.zero;

        // 방 이름에 따라 목표 위치와 회전값 설정
        switch (roomName)
        {
            case "NTH313":
                targetPosition = new Vector3(27, -2, -33);
                targetRotation = new Vector3(0, 218, 0);
                break;
            case "NTH311":
                targetPosition = new Vector3(26, -0.7f, -9);
                targetRotation = new Vector3(0, 434, 0);
                break;
            case "Hallway":
                targetPosition = new Vector3(18.58f, -0.72f, -16.8f);
                targetRotation = new Vector3(0, -38.51f, 0);
                break;
            case "ANH313":
                targetPosition = new Vector3(2,-0.9f, 9);
                targetRotation = new Vector3(0, -176, 0);
                break;
            default:
                Debug.LogError("Unknown room name: " + roomName);
                return;
        }

        // 카메라의 위치를 목표 위치로 이동
        transform.position = targetPosition;

        // 카메라의 회전을 목표 회전으로 설정
        transform.eulerAngles = targetRotation;
    }
}
