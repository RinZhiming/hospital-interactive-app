using System;
using Fusion;
using UnityEngine;

public class PlayerInteract : NetworkBehaviour
{
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private CharacterController characterController;
    private Collider hitObject;

    public override void Spawned()
    {
        if (!HasStateAuthority) return;
        if (PlayerManager.IsPause || NurseManager.IsDialogue) return;
        
        hitObject = null;
    }

    public override void Render()
    {
        if (!HasStateAuthority) return;
        
        if (PlayerManager.IsPause || NurseManager.IsDialogue) return;

        hitObject = Physics.SphereCast(
            transform.position + characterController.center,
            characterController.height/2,
            transform.forward, 
            out RaycastHit hit, 
            1.5f, 
            detectLayer) ? hit.collider : null;

        if (hitObject is not null)
        {
            if (hitObject.gameObject.CompareTag("OtherPlayer"))
            {
                PlayerEvent.OnTargetPlayer?.Invoke(hitObject.gameObject);
                return;
            }
        }

        PlayerEvent.OnUnTargetPlayer?.Invoke();
    }

    public Collider HitObject
    {
        get => hitObject;
        set => hitObject = value;
    }
}