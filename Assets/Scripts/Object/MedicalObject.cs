using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MedicalObject : MonoBehaviour
{
    [SerializeField] private InputField nameMedicalInputField, numberMedicalInputField, costMedicalInputField;
    public Medical Medical { get; set; } = new();

    public InputField NameMedicalInputField
    {
        get => nameMedicalInputField;
        set => nameMedicalInputField = value;
    }

    public InputField NumberMedicalInputField
    {
        get => numberMedicalInputField;
        set => numberMedicalInputField = value;
    }

    public InputField CostMedicalInputField
    {
        get => costMedicalInputField;
        set => costMedicalInputField = value;
    }
}