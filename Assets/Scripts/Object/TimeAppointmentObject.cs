using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class TimeAppointmentObject : MonoBehaviour
{
    [SerializeField] private Text dateText, timeText;
    [SerializeField] private Button button;
    public static Action<TimeAppointmentObject> OnClick;
    public AppointmentClone Appointment { get; set; }

    private void Awake()
    {
        button.onClick.AddListener(ToggleClick);
    }

    private void ToggleClick()
    {
        OnClick(this);
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
    
    public Button Button
    {
        get => button;
        set => button = value;
    }
}