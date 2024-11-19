using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppointmentController : MonoBehaviour
{
    private AppointmentModel model;

    private void Awake()
    {
        model = new();
        
        AppointmentEvent.OnExitRoom += OnExitRoom;
    }

    private void Start()
    {
        model.Appointment = null;
        
    }

    private void OnDestroy()
    {
        AppointmentEvent.OnExitRoom -= OnExitRoom;
    }
    
    private void OnExitRoom()
    {
        PhotonChatEvent.OnDisconnect?.Invoke();
        GameManager.StartShared("Main", 3);
    }
}
