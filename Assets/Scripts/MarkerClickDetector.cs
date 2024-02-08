/*using UnityEngine;

public class MarkerClickDetector : MonoBehaviour
{
    private Camera currentCamera; // ���� Ȱ��ȭ�� ī�޶� ������ ����
    public GameObject informationPanel; // Ȱ��ȭ�� ���� �г�
    public Transform markerTransform; // Ŭ�� ������ ���� ��Ŀ ������Ʈ�� Transform

    void Update()
    {
        // ���� Ȱ��ȭ�� ī�޶� ã���ϴ�.
        currentCamera = FindActiveCamera();

        if (currentCamera != null && Input.GetMouseButtonDown(0)) // ���콺 ��Ŭ�� ����
        {
            Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // Ŭ���� ������Ʈ�� ��Ŀ ������Ʈ���� Ȯ���մϴ�.
                if (hit.transform == markerTransform)
                {
                    informationPanel.SetActive(true);
                }
            }
        }
    }

    Camera FindActiveCamera()
    {
        // ��� ī�޶� ��ȸ�ϸ� ���� Ȱ��ȭ�� ī�޶� ã���ϴ�.
        foreach (Camera cam in Camera.allCameras)
        {
            if (cam.gameObject.activeSelf && cam.enabled)
            {
                return cam;
            }
        }
        return null; // Ȱ��ȭ�� ī�޶� ������ null�� ��ȯ�մϴ�.
    }
}
*/
using UnityEngine;

public class MarkerClickDetector : MonoBehaviour
{
    public GameObject informationPanel; // Ȱ��ȭ�� ���� �г�

    void Start()
    {
        // �ʿ��� ��� ���⿡ �ʱ�ȭ �ڵ带 �߰�
    }

    void OnMouseDown()
    {
        // ��Ŀ Ŭ�� �� ���� �г��� Ȱ��ȭ�մϴ�.
        if (informationPanel != null)
        {
            informationPanel.SetActive(true);
        }
    }
}