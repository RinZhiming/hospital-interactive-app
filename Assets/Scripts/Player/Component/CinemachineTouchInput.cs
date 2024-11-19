using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using UnityEngine;

public class CinemachineTouchInput : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private TouchInputRotate touch;
    [SerializeField] private float mouseSpeed;
    [SerializeField] private Transform cameraTransform;
    private CinemachinePOV pov;

    private void Start()
    {
        if (PlayerManager.IsPause || NurseManager.IsDialogue) return;
        
        pov = camera.GetCinemachineComponent<CinemachinePOV>();
    }

    private void Update()
    {
        if (PlayerManager.IsPause || NurseManager.IsDialogue) return;
        
        pov.m_HorizontalAxis.Value += touch.InputDistance.x * mouseSpeed * Time.deltaTime;
        pov.m_VerticalAxis.Value -= touch.InputDistance.y * mouseSpeed * Time.deltaTime;
    }

    public Transform CameraTransform
    {
        get => cameraTransform;
        set => cameraTransform = value;
    }
    
    public CinemachineVirtualCamera VirtualCamera
    {
        get => camera;
        set => camera = value;
    }
}
