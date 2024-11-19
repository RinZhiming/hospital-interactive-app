using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Fusion;
using UnityEngine;

public class DoctorPlayer : NetworkPlayer
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
    private bool isPause;
    
    public override void Spawned()
    {
        base.Spawned();
        if (!HasStateAuthority) return;
        
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        
        playerAnimator.Animator.SetBool("Grounded", true);
        playerAnimator.Animator.SetFloat("MoveSpeed", 0);
        
        cameraTransform = FindObjectOfType<CinemachineTouchInput>().CameraTransform;
        virtualCamera = FindObjectOfType<CinemachineTouchInput>().VirtualCamera;
        FindObjectOfType<CinemachineTouchInput>().VirtualCamera.Follow = lookAt;
        //FindFirstObjectByType<PlayerMinimapManager>().Player = gameObject.transform;
        
        gameObject.tag = "Doctor";
    }
    
    public override void Render()
    {
        
        if (!HasStateAuthority) return;
        
        if (!isSpawn) return;
        
        if(auth != null) NetworkPlayerId.PlayerId = auth.CurrentUser.UserId;
        
        if (PlayerManager.IsPause) return;
        
        
        
        Rotate();
        Movement();
        virtualCamera.enabled = true;
        
        Pause();
        Gravity();
    }

    private void Pause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            isPause = !isPause;
        
        Cursor.visible = isPause;
        Cursor.lockState = isPause ? CursorLockMode.None :CursorLockMode.Locked;
    }

    public override void FixedUpdateNetwork()
    {
        Animation();
    }

    private void Movement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

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
}
