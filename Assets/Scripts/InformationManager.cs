using UnityEngine;
using TMPro;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;

public class InformationManager : MonoBehaviour
{
    public GameObject informationPanel; // Information Panel
    public GameObject addInformationPanel; // Information �߰� �г�
    public TextMeshProUGUI informationText; // Information�� ǥ���ϴ� Text ������Ʈ
    private FirebaseFirestore db;
    public string selectedMarkerId; // ���� ���õ� ��Ŀ�� ID

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
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
    public void SetSelectedMarkerId(string id)
    {
        selectedMarkerId = id;
    }
    public void OnCloseButtonClicked()
    {
        informationPanel.SetActive(false); // X ��ư Ŭ�� �� Information Panel ��Ȱ��ȭ
    }

    public void AddInformation(string info)
    {
        informationText.text += info + "\n"; // ���� �߰�
        if (string.IsNullOrEmpty(selectedMarkerId))
        {
            Debug.LogError("Selected marker ID is not set or empty.");
            return; // ��ȿ���� ���� ��� �߰� ���� ����
        }
        // Firestore�� ���� �߰�
        DocumentReference docRef = db.Collection("markers").Document(selectedMarkerId);
        Dictionary<string, object> update = new Dictionary<string, object>
        {
            { "information", informationText.text }
        };
        docRef.SetAsync(update, SetOptions.MergeAll);
        db.Collection("markers").Document(selectedMarkerId).SetAsync(update, SetOptions.MergeAll);
    }
    public void DisplayInformation(string info)
    {
        informationText.text = info;
    }
}
