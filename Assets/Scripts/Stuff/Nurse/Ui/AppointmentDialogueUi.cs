using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class NurseAppointmentManager
{
    [SerializeField] private Button 
        appointmentButton, 
        appointmentHomeButton,
        doctorBackButton,
        timeBackButton,
        confirmBackButton,
        doctorNextButton,
        timeNextButton,
        confirmButton;

    [SerializeField] private CanvasGroup
        mainDialogueGroup,
        appointmentDialogueGroup,
        doctorCanvasGroup,
        timeCanvasGroup,
        confirmCanvasGroup,
        successConfirmCanvasGroup;

    [SerializeField] private GameObject doctorPrefab, datePrefab;
    [SerializeField] private Transform doctorContainer, dateContainer;
    [SerializeField] private Sprite[] icons;
    [SerializeField] private Text doctorNameText, dateText, timeText;
}
