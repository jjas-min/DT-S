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
    void Start()
    {
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
        // ī�޶󿡼� ��Ʈ ����Ʈ �������� ���� �� ����� ��ġ�� ǥ���� ��ġ�մϴ�.
        Vector3 directionToCamera = (currentCamera.transform.position - hitPoint).normalized; // ī�޶� ������ ���� ����
        float offsetDistance = 0.1f; // ī�޶� �������� �󸶳� �ڷ� ���������� ���� �Ÿ�
        Vector3 placePosition = hitPoint + directionToCamera * offsetDistance; // ���� ǥ���� ��ġ�� ��ġ

        GameObject markerInstance = Instantiate(markerPrefab, placePosition, Quaternion.identity); // ǥ���� �����մϴ�.
        MarkerClickDetector clickDetector = markerInstance.AddComponent<MarkerClickDetector>();
        clickDetector.informationPanel = informationPanel;
        selectionUI.SetActive(false); // ǥ�� ���� �� UI �г��� ��Ȱ��ȭ�մϴ�.
    }

}
