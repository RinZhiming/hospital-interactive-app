using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeDoctorTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Player>().IsOwner)
        {
            SeeDoctorEvent.OnPlayerTrigger?.Invoke();
        }
    }
}
