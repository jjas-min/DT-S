using System;
using System.Collections;
using System.Collections.Generic; // 이 줄을 추가합니다.
using Firebase;
using Firebase.Firestore;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseTimeRecorder : MonoBehaviour
{
    private FirebaseFirestore db;

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                db = FirebaseFirestore.DefaultInstance;
                StartCoroutine(RecordTimeEverySecond());
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
            }
        });
    }

    IEnumerator RecordTimeEverySecond()
    {
        while (true)
        {
            SaveTimeToFirestore(DateTime.Now);
            yield return new WaitForSeconds(1f);
        }
    }

    void SaveTimeToFirestore(DateTime time)
    {
        var timeData = new Dictionary<string, object>
        {
            { "timestamp", time.ToString("yyyy-MM-dd HH:mm:ss") }
        };

        db.Collection("time").AddAsync(timeData).ContinueWithOnMainThread(task =>
        {
            if (task.Exception != null)
            {
                Debug.LogError($"Error adding document to Firestore: {task.Exception}");
            }
            else
            {
                Debug.Log("Time recorded successfully.");
            }
        });
    }
}