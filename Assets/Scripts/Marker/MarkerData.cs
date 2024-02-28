using UnityEngine;
using System;

public class MarkerData : MonoBehaviour
{
    public string id;
    public Vector3 position;
    public string information;
    public int level;
    public DateTime creationTime;
    public string location;
    public bool isSolved;


    public void SetData(string id, Vector3 position, string information, int level, DateTime creationTime, string location, bool isSolved)
    {
        this.id = id;
        this.position = position;
        this.information = information;
        this.level = level;
        this.creationTime = creationTime;
        this.location = location;
        this.isSolved = isSolved;
    }
}
