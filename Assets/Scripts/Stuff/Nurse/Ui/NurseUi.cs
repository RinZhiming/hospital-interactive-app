using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class NurseManager
{
    [SerializeField] private Button
        interactButton,
        seeDoctorButton,
        appointmentButton,
        medicineButton,
        backButton;
    
    [SerializeField]
    private CanvasGroup 
        mainDialogueGroup, 
        seeDoctorDialogueGroup, 
        appointmentDialogueGroup, 
        medicineDialogueGroup;
}
