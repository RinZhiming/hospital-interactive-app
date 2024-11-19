using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public partial class NurseHistoryManager
{
    [SerializeField] private Text patientNameText, dateText, doctorNameText, pbText, totalText;
    [SerializeField] private CanvasGroup medicalCanvasGroup, doctorCanvasGroup, dateCanvasGroup, historyCanvasGroup;
    [SerializeField] private Button medicalBackButton, closeButton, historyButton, dateBackButton, doctorBackButton;
    [SerializeField] private Transform medicalContainer, doctorContainer,dateContainer;
    [SerializeField] private GameObject medicalPrefab, doctorPrefab, datePrefab;
    [SerializeField] private Sprite[] icons;
}
