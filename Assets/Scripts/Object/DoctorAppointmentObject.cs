using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DoctorAppointmentObject : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Image icon, highlight;
    [SerializeField] private Toggle toggle;
    private DoctorData doctor;

    public static Action<DoctorAppointmentObject, bool> OnToggle;

    private void Awake()
    {
        toggle.onValueChanged.AddListener(b =>
        {
            ToggleClick(b);
            highlight.gameObject.SetActive(b);
        });
    }

    private void Start()
    {
        highlight.gameObject.SetActive(false);
    }

    private void ToggleClick(bool toggle)
    {
        OnToggle(this, toggle);
    }
    
    public DoctorData Doctor
    {
        get => doctor;
        set => doctor = value;
    }
    
    public Text NameText
    {
        get => nameText;
        set => nameText = value;
    }

    public Image Icon
    {
        get => icon;
        set => icon = value;
    }
    
    public Toggle Toggle
    {
        get => toggle;
        set => toggle = value;
    }
}
