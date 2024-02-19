using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5.0f; // 플레이어 이동 속도

    public float rotateSpeed = 100.0f; // 회전 속도 조절 변수
    private Vector3 prevMousePosition; // 이전 마우스 위치 저장 변수

    void Update()
    {
        float x = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime; // A, D 키 혹은 좌우 화살표
        float z = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime; // W, S 키 혹은 상하 화살표

        // 플레이어 이동
        transform.Translate(x, 0, z);

        // 'e' 키를 누를 때
        if (Input.GetKey(KeyCode.E))
        {
            MoveUp();
        }

        // 'q' 키를 누를 때
        if (Input.GetKey(KeyCode.Q))
        {
            MoveDown();
        }
    }

    void MoveUp()
    {
        // 현재 위치에서 y좌표를 올림
        transform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
    }

    void MoveDown()
    {
        // 현재 위치에서 y좌표를 내림
        transform.position -= new Vector3(0, moveSpeed * Time.deltaTime, 0);
    }
}
