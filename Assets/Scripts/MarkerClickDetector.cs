/*using UnityEngine;

public class MarkerClickDetector : MonoBehaviour
{
    private Camera currentCamera; // 현재 활성화된 카메라를 저장할 변수
    public GameObject informationPanel; // 활성화할 정보 패널
    public Transform markerTransform; // 클릭 감지를 위한 마커 오브젝트의 Transform

    void Update()
    {
        // 현재 활성화된 카메라를 찾습니다.
        currentCamera = FindActiveCamera();

        if (currentCamera != null && Input.GetMouseButtonDown(0)) // 마우스 좌클릭 감지
        {
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // 클릭된 오브젝트가 마커 오브젝트인지 확인합니다.
                if (hit.transform == markerTransform)
                {
                    informationPanel.SetActive(true);
                }
            }
        }
    }

    Camera FindActiveCamera()
    {
        // 모든 카메라를 순회하며 현재 활성화된 카메라를 찾습니다.
        foreach (Camera cam in Camera.allCameras)
        {
            if (cam.gameObject.activeSelf && cam.enabled)
            {
                return cam;
            }
        }
        return null; // 활성화된 카메라가 없으면 null을 반환합니다.
    }
}
*/
using UnityEngine;

public class MarkerClickDetector : MonoBehaviour
{
    public GameObject informationPanel; // 활성화할 정보 패널

    void Start()
    {
        // 필요한 경우 여기에 초기화 코드를 추가
    }

    void OnMouseDown()
    {
        // 마커 클릭 시 정보 패널을 활성화합니다.
        if (informationPanel != null)
        {
            informationPanel.SetActive(true);
        }
    }
}