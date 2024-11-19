using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateTimeAppointmentObject : MonoBehaviour
{
    [SerializeField] private Text dateText, timeText;
    [SerializeField] private Toggle toggle;
    [SerializeField] private Image highlight;
    public AppointmentClone Appointment { get; set; }
    public static Action<DateTimeAppointmentObject, bool> OnToggle;

    private void Awake()
    {
        highlight.gameObject.SetActive(false);
    }

    private void Start()
    {
        toggle.onValueChanged.AddListener(b =>
        {
            ToggleClick(b);
            highlight.gameObject.SetActive(b);
        });
    }

    private void ToggleClick(bool toggle)
    {
        OnToggle(this, toggle);
    }

    public Text DateText
    {
        get => dateText;
        set => dateText = value;
    }

    public Text TimeText
    {
        get => timeText;
        set => timeText = value;
    }
    
    public Toggle Toggle
    {
        get => toggle;
        set => toggle = value;
    }
}
