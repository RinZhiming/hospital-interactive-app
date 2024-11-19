using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HistoryDoctorObject : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    public DoctorData Doctor { get; set; }
    public static Action<HistoryDoctorObject> OnClick;

    private void Awake()
    {
        button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        OnClick(this);
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
