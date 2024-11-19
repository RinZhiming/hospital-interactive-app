using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class NurseTrigger : MonoBehaviour
{
    [SerializeField] private Nurse nurse;
    public static Action<Nurse, Collider> OnTriggerEnterEvent;
    public static Action<Collider> OnTriggerExitEvent;
    public static Action<Nurse, Collider> OnTriggerStayEvent;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Player>().IsOwner)
            OnTriggerEnterEvent?.Invoke(nurse, other);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<Player>().IsOwner)
            OnTriggerStayEvent?.Invoke(nurse, other);
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<Player>().IsOwner)
            OnTriggerExitEvent?.Invoke(other);
    }
}
