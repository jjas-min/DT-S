﻿using UnityEngine;

public class FirstPersonViewCameraDragRotate : MonoBehaviour
{
    public float rotateSpeed = 100.0f; // 회전 속도 조절 변수
    private Camera attachedCamera; // 현재 스크립트가 부착된 카메라
    private Vector3 prevMousePosition; // 이전 마우스 위치 저장 변수

    void Start()
    {
        // 현재 스크립트가 부착된 카메라 컴포넌트를 가져옵니다.
        attachedCamera = GetComponent<Camera>();
    }

    void Update()
    {
        // 카메라가 활성화되어 있지 않으면 회전 처리를 하지 않습니다.
        if (!attachedCamera.enabled) return;

        // 마우스 버튼(일반적으로 왼쪽 버튼)이 눌렸는지 확인
        if (Input.GetMouseButtonDown(0))
        {
            // 마우스 버튼이 처음 눌렸을 때, 현재 마우스 위치 저장
            prevMousePosition = Input.mousePosition;
        }

        // 마우스 버튼이 눌린 상태에서 움직이고 있는지 확인
        if (Input.GetMouseButton(0))
        {
            // 현재 마우스 위치와 이전 마우스 위치의 차이 계산
            Vector3 delta = Input.mousePosition - prevMousePosition;
            // Y축 회전은 마우스의 X축 움직임에, X축 회전은 마우스의 Y축 움직임에 반응하도록 설정
            float rotateY = delta.x * rotateSpeed * Time.deltaTime;
            float rotateX = -delta.y * rotateSpeed * Time.deltaTime;

            // 현재 오브젝트(카메라)의 회전에 마우스 움직임에 따른 회전값 적용
            transform.eulerAngles += new Vector3(rotateX, rotateY, 0);

            // 현재 마우스 위치를 이전 마우스 위치로 업데이트
            prevMousePosition = Input.mousePosition;
        }
    }
}
