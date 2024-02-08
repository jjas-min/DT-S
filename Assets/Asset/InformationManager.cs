using UnityEngine;
using TMPro;

public class InformationManager : MonoBehaviour
{
    public GameObject informationPanel; // Information Panel
    public GameObject addInformationPanel; // Information 추가 패널
    public TextMeshProUGUI informationText; // Information을 표시하는 Text 컴포넌트

    void Start()
    {
        informationPanel.SetActive(false); // 시작 시 Information Panel을 비활성화
        addInformationPanel.SetActive(false); // 시작 시 Add Information Panel을 비활성화
    }

    public void OnAddButtonClicked()
    {
        addInformationPanel.SetActive(true); // Add 버튼 클릭 시 Information 추가 패널 활성화
    }

    public void OnDeleteButtonClicked()
    {
        // Information Text에서 마지막 줄 삭제
        if (!string.IsNullOrEmpty(informationText.text))
        {
            string[] lines = informationText.text.Split('\n');
            if (lines.Length > 1)
            {
                informationText.text = string.Join("\n", lines, 0, lines.Length - 2) + "\n";
            }
            else
            {
                informationText.text = "";
            }
        }
    }

    public void OnCloseButtonClicked()
    {
        informationPanel.SetActive(false); // X 버튼 클릭 시 Information Panel 비활성화
    }

    public void AddInformation(string info)
    {
        informationText.text += info + "\n"; // 정보 추가
    }
}
