using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class NurseSeeDoctorManager
{
    [SerializeField] private Button
        changeTimeBackButton, 
        confirmChangeTimeBackButton,
        yesDoctorButton,
        changeTimeButton,
        changeTimeNextButton,
        confirmChangeTimeButton,
        gotoDoctorButton,
        enterRoomButton,
        waitEnterRoomButton;
    
    [SerializeField] private Button[] closeButtons;
    
    [SerializeField] private GameObject appointmentPrefab;

    [SerializeField] private CanvasGroup 
        mainSeeDoctorGroup,
        seeDoctorGroup,
        confirmSeeDoctorGroup,
        changeTimeGroup,
        confirmChangeTimeGroup,
        mainCanvasGroup,
        enterRoomGroup;

    [SerializeField] private Text 
        seeDoctorText, 
        confirmSeeDoctorText, 
        doctorText, 
        dateText, 
        timeText;
    
    [SerializeField] private Transform timeAppointment;
}
