using System;
using UnityEngine;
using UnityEngine.UI;

public class HistoryPatientObject : MonoBehaviour
{
    [SerializeField] private Text dateText, nameText;
    [SerializeField] private Button buttons;
    [SerializeField] private Image icon;

    public static Action<HistoryPatientObject> OnClick;
    public AppointmentClone Appointment { get; set; }

    private void Start()
    {
        buttons.onClick.AddListener(AppointmentClick);
    }

    private void AppointmentClick()
    {
        OnClick?.Invoke(this);
    }

    public Text DateText
    {
        get => dateText;
        set => dateText = value;
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
}