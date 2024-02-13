using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class MarkerData
{
    public string id;
    public Vector3 position;
    public string information;
    public int level;
    public DateTime creationTime; // ���� �ð�

    public MarkerData(Vector3 position, string information, int level)
    {
        this.id = System.Guid.NewGuid().ToString();
        this.position = position;
        this.information = information;
        this.level = level;
        this.creationTime = DateTime.UtcNow; // ���� UTC �ð�
    }
}

