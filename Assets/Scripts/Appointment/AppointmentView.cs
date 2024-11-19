using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public partial class AppointmentView : MonoBehaviour
{
    private AppointmentClone appointment;
    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    private void Awake()
    {
        exitRoomButton.onClick.AddListener(ExitRoomButton);
        exitRoomButton.interactable = false;
        
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        
        appointment = NurseSeeDoctorManager.CurrentAppointment;
        GetPlayerCharacter();
        OnEnterRoom();
    }

    private void GetPlayerCharacter()
    {
        dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("userdata").GetValueAsync()
            .ContinueWithOnMainThread(result =>
            {
                if (result.IsCanceled || result.IsFaulted)
                {
                    Debug.LogError(result.Exception.Message);
                    return;
                }

                if (result.IsCompleted)
                {
                    var profile = JsonUtility.FromJson<UserData>(result.Result.GetRawJsonValue());
                    Instantiate(players[profile.iconProfile], playerPos.position, playerPos.rotation);
                    GetDoctorCharacter();
                }
            });
    }
    
    private void GetDoctorCharacter()
    {
        dbRef.Child("doctors").Child(appointment.doctorId).GetValueAsync()
            .ContinueWithOnMainThread(result =>
            {
                if (result.IsCanceled || result.IsFaulted)
                {
                    Debug.LogError(result.Exception.Message);
                    return;
                }

                if (result.IsCompleted)
                {
                    var profile = JsonUtility.FromJson<DoctorData>(result.Result.GetRawJsonValue());
                    Instantiate(doctors[profile.iconProfile - 6], doctorPos.position, doctorPos.rotation);
                }
            });
    }
    
    private void OnEnterRoom()
    {
        exitRoomButton.interactable = true;                                                 
    }
    
    private void ExitRoomButton()
    {
        try
        {
            exitRoomButton.interactable = false;
            AppointmentEvent.OnExitRoom?.Invoke();
        }
        catch (Exception e)
        {
            exitRoomButton.interactable = true;
        }
    }
}
