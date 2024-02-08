using UnityEngine;
using TMPro;

public class InformationManager : MonoBehaviour
{
    public GameObject informationPanel; // Information Panel
    public GameObject addInformationPanel; // Information �߰� �г�
    public TextMeshProUGUI informationText; // Information�� ǥ���ϴ� Text ������Ʈ

    void Start()
    {
        informationPanel.SetActive(false); // ���� �� Information Panel�� ��Ȱ��ȭ
        addInformationPanel.SetActive(false); // ���� �� Add Information Panel�� ��Ȱ��ȭ
    }

    public void OnAddButtonClicked()
    {
        addInformationPanel.SetActive(true); // Add ��ư Ŭ�� �� Information �߰� �г� Ȱ��ȭ
    }

    public void OnDeleteButtonClicked()
    {
        // Information Text���� ������ �� ����
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
        informationPanel.SetActive(false); // X ��ư Ŭ�� �� Information Panel ��Ȱ��ȭ
    }

    public void AddInformation(string info)
    {
        informationText.text += info + "\n"; // ���� �߰�
    }
}
