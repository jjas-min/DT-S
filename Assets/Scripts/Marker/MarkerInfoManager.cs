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
                informationPanel.SetActive(false); 
            }
            else
            {
                Debug.LogError("Error deleting marker from Firestore."); 
            }
        });
    }


    public void OnCloseButtonClicked() //Close Info
    {
        informationPanel.SetActive(false); 
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

        informationText.text = $"Information: {markerData.information}";
        levelText.text = $"Level: {markerData.level}";
        timestampText.text = $"Created: {markerData.creationTime.ToString("yyyy-MM-dd HH:mm:ss")}";
        locationText.text = $"Location: {markerData.location}";

        informationPanel.SetActive(true);
    }

    public void OnEditButtonClicked()
    {
        markerEditPanel.SetActive(true);
        EditInformation();
    }

    IEnumerator WaitAndDisplayInformation(float waitTime)
    {
        yield return new WaitForSeconds(waitTime); 
        DisplayInformation(); 
    }

    public void EditInformation()
    {
        EditInformationInputField.text = markerData.information;
        EditLevelText.value = markerData.level-1;

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

            StartCoroutine(WaitAndDisplayInformation(0.025f));
        });
        cancelButton.onClick.RemoveAllListeners();
        cancelButton.onClick.AddListener(() =>
        {
            markerEditPanel.SetActive(false);
        });
    }
}
