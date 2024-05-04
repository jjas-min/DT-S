﻿using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ViewSwitcher : MonoBehaviour
{
    public Camera topViewCamera; // 탑뷰 카메라
    public Camera firstPersonCamera; // 1인칭 뷰 카메라
    public TMP_Dropdown roomsDropdown;

    void Start()
    {
        // 시작 시 1인칭 뷰 활성화 및 오디오 리스너 설정
        ActivateCamera(firstPersonCamera);
        roomsDropdown.value = 0; // Hallway 선택

        // 드랍다운 값 변경 이벤트에 MoveToRoom 함수 연결
        roomsDropdown.onValueChanged.AddListener(delegate {
            DropdownValueChanged(roomsDropdown);
        });
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        // 선택된 방 이름 가져오기
        string selectedRoom = change.options[change.value].text;
        // 선택된 방으로 이동
        SwitchToFirstPersonView();
        MoveToRoom(selectedRoom);
    }

    // 탑뷰로 전환하는 함수
    public void SwitchToTopView()
    {
        ActivateCamera(topViewCamera);
    }

    // 1인칭 뷰로 전환하는 함수
    public void SwitchToFirstPersonView()
    {
        ActivateCamera(firstPersonCamera);
    }

    // 지정된 카메라 활성화 및 오디오 리스너 설정
    private void ActivateCamera(Camera cameraToActivate)
    {
        // 모든 카메라 비활성화
        topViewCamera.enabled = false;
        firstPersonCamera.enabled = false;

        // 모든 오디오 리스너 비활성화
        AudioListener[] allListeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener listener in allListeners)
        {
            listener.enabled = false;
        }

        // 지정된 카메라와 오디오 리스너 활성화
        cameraToActivate.enabled = true;
        AudioListener audioListener = cameraToActivate.GetComponent<AudioListener>();
        if (audioListener != null)
        {
            audioListener.enabled = true;
        }
    }

    public void MoveToRoom(string roomName)
    {
        Vector3 targetPosition = Vector3.zero;
        Vector3 targetRotation = Vector3.zero;

        // 방 이름에 따라 목표 위치와 회전값 설정
        switch (roomName)
        {
            case "NTH313":
                targetPosition = new Vector3(-1.9f, -1.7f, -26.6f);
                targetRotation = new Vector3(0, 139.7f, 0);
                break;
            case "NTH311":
                targetPosition = new Vector3(-19.6f, -1.75f, -39.1f);
                targetRotation = new Vector3(0, 287.4f, 0);
                break;
            case "Hallway":
                targetPosition = new Vector3(-7.9f, -1.7f, -24.3f);
                targetRotation = new Vector3(0, 20.2f, 0);
                break;
            case "ANH313":
                targetPosition = new Vector3(7.6f, -2.6f, -3);
                targetRotation = new Vector3(1.35f, 56.1f, 358.5f);
                break;
            case "Top View": // Top View 시점으로 변경
                ActivateCamera(topViewCamera);
                return;
            default:
                Debug.LogError("Unknown room name: " + roomName);
                return;
        }

        // 카메라의 위치를 목표 위치로 이동
        firstPersonCamera.transform.position = targetPosition;

        // 카메라의 회전을 목표 회전으로 설정
        firstPersonCamera.transform.eulerAngles = targetRotation;
    }
}
