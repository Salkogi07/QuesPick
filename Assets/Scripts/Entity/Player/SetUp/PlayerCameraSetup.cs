using System;
using Unity.Netcode;
using Unity.Cinemachine;
using UnityEngine;


public class PlayerCameraSetup : NetworkBehaviour
{
    private CinemachineCamera _camera;

    //싱글에서 테스트 용 (나중에 지우기)
    public void Start()
    {
        // 씬에 있는 Cinemachine 카메라를 찾습니다.
        GameObject camObj = GameObject.FindGameObjectWithTag("CinemachineCamera");
        if (camObj != null)
        {
            _camera = camObj.GetComponent<CinemachineCamera>();
            if (_camera != null)
            {
                // 찾은 카메라가 자신의 캐릭터를 따라다니도록 설정합니다.
                _camera.Follow = transform;
                _camera.LookAt = transform;
                Debug.Log("Cinemachine camera target set for local player.");
            }
            else
            {
                Debug.LogError("Object with tag 'CinemachineCamera' does not have a CinemachineCamera component.");
            }
        }
        else
        {
            Debug.LogError("Could not find GameObject with tag 'CinemachineCamera'. Make sure your scene camera has this tag.");
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            // 씬에 있는 Cinemachine 카메라를 찾습니다.
            GameObject camObj = GameObject.FindGameObjectWithTag("CinemachineCamera");
            if (camObj != null)
            {
                _camera = camObj.GetComponent<CinemachineCamera>();
                if (_camera != null)
                {
                    // 찾은 카메라가 자신의 캐릭터를 따라다니도록 설정합니다.
                    _camera.Follow = transform;
                    _camera.LookAt = transform;
                    Debug.Log("Cinemachine camera target set for local player.");
                }
                else
                {
                    Debug.LogError("Object with tag 'CinemachineCamera' does not have a CinemachineCamera component.");
                }
            }
            else
            {
                Debug.LogError("Could not find GameObject with tag 'CinemachineCamera'. Make sure your scene camera has this tag.");
            }
        }
    }
}