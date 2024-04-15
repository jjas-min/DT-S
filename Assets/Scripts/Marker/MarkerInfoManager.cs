using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Firebase.Firestore;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Extensions;

public class MarkerInfoManager : MonoBehaviour
{
    public GameObject informationPanel;
    public GameObject markerEditPanel;
    // Camera
    public GameObject firstPersonView;

    public Button confirmButton;
    public Button cancelButton;

    private FirebaseFirestore db;

    public TMP_Text informationText;
    public TMP_InputField EditInformationInputField;
    public TMP_Text levelText;
    public TMP_Dropdown EditLevelText;
    public TMP_Text timestampText;
    public TMP_Text locationText;

    private MarkerData markerData;
    private string id;

    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        informationPanel.SetActive(false);
        markerEditPanel.SetActive(false);
    }

    public void OnDeleteButtonClicked() //Delete Info
    {
        db.Collection("Markers").Document(id).DeleteAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"Marker {id} deleted successfully from Firestore.");
                markerEditPanel.SetActive(false);
            }
            else
            {
                Debug.LogError("Error deleting marker from Firestore.");
            }
        });
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
    }


    public void OnCloseButtonClicked() //Close Info
    {
        informationPanel.SetActive(false);
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
    }

    public void SetInformation(MarkerData markerData)
    {
        this.markerData = markerData;
        id = markerData.id;
        DisplayInformation();
    }

    public void DisplayInformation()
    {
        informationPanel.SetActive(false);

        informationText.text = $"{markerData.information}";
        levelText.text = $"{markerData.level}";
        timestampText.text = $"{markerData.creationTime.ToString("yyyy-MM-dd hh:mm tt")}";
        locationText.text = $"{markerData.location}";

        // 레벨에 따라 텍스트 색상 변경
        switch (markerData.level)
        {
            case 1:
                levelText.color = Color.red;
                break;
            case 2:
                levelText.color = Color.yellow;
                break;
            case 3:
                levelText.color = Color.green;
                break;
            default:
                levelText.color = Color.white;
                break;
        }

        informationPanel.SetActive(true);
    }

    public void OnEditButtonClicked()
    {
        markerEditPanel.SetActive(true);
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = false;
        EditInformation();
    }

    public void OnSolvedButtonClicked()
    {
        Dictionary<string, object> solvedUpdate = new Dictionary<string, object>
    {
        {"isSolved", true}
    };

        db.Collection("Markers").Document(id).UpdateAsync(solvedUpdate).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted && !task.IsFaulted)
            {
                Debug.Log($"Marker {id} marked as solved successfully in Firestore.");
            }
            else
            {
                Debug.LogError("Error marking marker as solved in Firestore: " + task.Exception.ToString());
            }
        });

        informationPanel.SetActive(false);
        firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
    }

    IEnumerator WaitAndDisplayInformation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        DisplayInformation();
    }

    public void EditInformation()
    {
        // Deactivate the information panel
        informationPanel.SetActive(false);

        EditInformationInputField.text = markerData.information;
        EditLevelText.value = markerData.level - 1;

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            Dictionary<string, object> updatedFields = new Dictionary<string, object>
            {
                {"information", EditInformationInputField.text},
                {"level", EditLevelText.value+1},
                {"creationTime", Timestamp.FromDateTime(DateTime.UtcNow)}
            };
            db.Collection("Markers").Document(id).UpdateAsync(updatedFields).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted && !task.IsFaulted)
                {
                    Debug.Log($"Marker {id} updated successfully in Firestore.");
                    markerEditPanel.SetActive(false);
                }
                else
                {
                    Debug.LogError("Error updating marker in Firestore: " + task.Exception.ToString());
                }
            });
            firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
            StartCoroutine(WaitAndDisplayInformation(0.025f));
        });
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            markerEditPanel.SetActive(false);
            firstPersonView.GetComponent<FirstPersonViewCameraController>().enabled = true;
        });
    }
}
