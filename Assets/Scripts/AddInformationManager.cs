using UnityEngine;
using TMPro;

public class AddInformationManager : MonoBehaviour
{
    public TMP_InputField inputField; // 사용자 입력 필드
    public InformationManager informationManager; // InformationManager 스크립트 참조
    public GameObject AddInformationPanel;

    public void OnConfirmButtonClicked()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            informationManager.AddInformation(inputField.text); // 정보 추가
            AddInformationPanel.SetActive(false); // 입력 패널 비활성화
            inputField.text = ""; // 입력 필드 초기화
        }
    }

    public void OnCancelButtonClicked()
    {
        AddInformationPanel.SetActive(false); // 취소 버튼 클릭 시 입력 패널 비활성화
        inputField.text = ""; // 입력 필드 초기화
    }
}
