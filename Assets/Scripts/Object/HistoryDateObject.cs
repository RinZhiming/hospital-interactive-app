using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HistoryDateObject : MonoBehaviour
{
    [SerializeField] private Text dateText, timeText;
    [SerializeField] private Button button;
    public MedicalSummaryClone MedicalSummary { get; set; }
    public static Action<HistoryDateObject> OnClick;

    private void Awake()
    {
        button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
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
}
