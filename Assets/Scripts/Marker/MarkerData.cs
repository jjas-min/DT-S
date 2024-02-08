using UnityEngine;

[System.Serializable]
public class MarkerData
{
    public string id;
    public Vector3 position;
    public string information;

    // ������: ��Ŀ�� ��ġ�� ������ �ʱ�ȭ�մϴ�.
    public MarkerData(Vector3 position, string information)
    {
        this.id = System.Guid.NewGuid().ToString(); // ������ ID�� �����մϴ�.
        this.position = position;
        this.information = information;
    }
}
