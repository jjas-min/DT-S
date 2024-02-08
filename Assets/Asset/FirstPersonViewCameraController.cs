using UnityEngine;

public class FirstPersonViewCameraController : MonoBehaviour
{
    public float speed = 5.0f; // 플레이어 이동 속도

    void Update()
    {
        float x = Input.GetAxis("Horizontal") * speed * Time.deltaTime; // A, D 키 혹은 좌우 화살표
        float z = Input.GetAxis("Vertical") * speed * Time.deltaTime; // W, S 키 혹은 상하 화살표

        // 플레이어 이동
        transform.Translate(x, 0, z);
    }
}
