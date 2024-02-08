using UnityEngine;

[System.Serializable]
public class MarkerData
{
    public string id;
    public Vector3 position;
    public string information;

    // 생성자: 마커의 위치와 정보를 초기화합니다.
    public MarkerData(Vector3 position, string information)
    {
        this.id = System.Guid.NewGuid().ToString(); // 고유한 ID를 생성합니다.
        this.position = position;
        this.information = information;
    }
}
