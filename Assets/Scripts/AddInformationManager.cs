using UnityEngine;
using TMPro;

public class AddInformationManager : MonoBehaviour
{
    public TMP_InputField inputField; // ����� �Է� �ʵ�
    public InformationManager informationManager; // InformationManager ��ũ��Ʈ ����
    public GameObject AddInformationPanel;

    public void OnConfirmButtonClicked()
    {
        if (!string.IsNullOrEmpty(inputField.text))
        {
            informationManager.AddInformation(inputField.text); // ���� �߰�
            AddInformationPanel.SetActive(false); // �Է� �г� ��Ȱ��ȭ
            inputField.text = ""; // �Է� �ʵ� �ʱ�ȭ
        }
    }

    public void OnCancelButtonClicked()
    {
        AddInformationPanel.SetActive(false); // ��� ��ư Ŭ�� �� �Է� �г� ��Ȱ��ȭ
        inputField.text = ""; // �Է� �ʵ� �ʱ�ȭ
    }
}
