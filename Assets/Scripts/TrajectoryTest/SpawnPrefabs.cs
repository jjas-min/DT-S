using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SpawnPrefabs : MonoBehaviour
{
    public GameObject prefab; // 배치할 프리팹

    void Start()
    {
        string filePath = "Assets/sfm_trajectory.txt"; // 파일 경로
        string[] lines = File.ReadAllLines(filePath); // 파일의 모든 라인 읽기

        // 상위 오브젝트 생성
        GameObject pointObject = new GameObject("Points");

        foreach (string line in lines)
        {
            string[] values = line.Split(' '); // 공백으로 구분된 값들 분리

            // x, y, z 좌표값 추출
            float x = (-1) * float.Parse(values[0]);
            float y = float.Parse(values[1]);
            float z = float.Parse(values[2]);

            // 좌표값을 Vector3로 변환하여 프리팹을 해당 위치에 배치
            Vector3 position = new Vector3(x, y, z);

            // 프리팹 생성 후 상위 오브젝트의 자식으로 추가
            GameObject instantiatedPrefab = Instantiate(prefab, position, Quaternion.identity);
            instantiatedPrefab.transform.parent = pointObject.transform;
        }
    }
}
