using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using UnityEngine;

public class Player : NetworkPlayer
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private NetworkMecanimAnimator playerAnimator;
    [SerializeField] private Transform lookAt;

    [SerializeField] private float walkSpeed, runSpeed, gravitySpeed, rotationSpeed, animationSpeed;
    
    private float speed, horizontal, vertical;
    private Vector3 movementDirection, playerVelocity, forwardDirection, rightDirection;
    private Quaternion freeRotate;
    private Transform cameraTransform;
    private CinemachineVirtualCamera virtualCamera;
    private FixedJoystick joystick;
    public bool IsOwner { get; private set; }

    public override void Spawned()
    {
        base.Spawned();
        
        if (!HasStateAuthority) return;

        IsOwner = true;
        
        if (PlayerManager.IsPause || NurseManager.IsDialogue) return;
        
        playerAnimator.Animator.SetBool("Grounded", true);
        playerAnimator.Animator.SetFloat("MoveSpeed", 0);
        
        cameraTransform = FindObjectOfType<CinemachineTouchInput>().CameraTransform;
        virtualCamera = FindObjectOfType<CinemachineTouchInput>().VirtualCamera;
        FindObjectOfType<CinemachineTouchInput>().VirtualCamera.Follow = lookAt;
        FindFirstObjectByType<PlayerMinimapManager>().Player = gameObject.transform;
        joystick = FindObjectOfType<FixedJoystick>();
        
        
        gameObject.tag = "Player";
    }

    public override void Render()
    {
        if (!HasStateAuthority) return;
        
        if (!isSpawn) return;
        
        if(auth != null) NetworkPlayerId.PlayerId = auth.CurrentUser.UserId;
        
        if (PlayerManager.IsPause || NurseManager.IsDialogue) return;
        
        
        Rotate();
        Movement();
        virtualCamera.enabled = true;

        Gravity();
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;
        
        Animation();
    }

    private void Movement()
    {
        horizontal = joystick.Horizontal;
        vertical = joystick.Vertical;

        movementDirection = forwardDirection * vertical + rightDirection * horizontal;

        characterController.Move(movementDirection * ((runSpeed - 0.5f) * Time.deltaTime));
    }

    private void Gravity()
    {
        if (characterController.isGrounded && playerVelocity.y < 1) playerVelocity.y = 0;
        playerVelocity.y -= gravitySpeed * Time.deltaTime;

        characterController.Move(playerVelocity * Time.deltaTime);
    }

    private void Rotate()
    {
        forwardDirection = cameraTransform.TransformDirection(Vector3.forward);
        forwardDirection.y = 0;

        rightDirection = cameraTransform.TransformDirection(Vector3.right);

        if (movementDirection.magnitude > 0.1f && movementDirection != Vector3.zero)
        {
            Vector3 lookDirection = movementDirection.normalized;
            freeRotate = Quaternion.LookRotation(lookDirection, transform.up);
            float diffRotate = freeRotate.eulerAngles.y - transform.eulerAngles.y;
            float eulerY = transform.eulerAngles.y;

            if (diffRotate < 0 || diffRotate > 0) eulerY = freeRotate.eulerAngles.y;
            Vector3 euler = new Vector3(0, eulerY, 0);

            transform.rotation =
                Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), rotationSpeed * Time.deltaTime);
        }
    }

    private void Animation()
    {
        playerAnimator.Animator.SetFloat("MoveSpeed", movementDirection.magnitude * 2);
    }
    
    public CinemachineVirtualCamera VirtualCamera
    {
        get => virtualCamera;
        set => virtualCamera = value;
    }
}
