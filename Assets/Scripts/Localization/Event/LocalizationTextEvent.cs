using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationTextEvent : MonoBehaviour
{
    [SerializeField, TextArea(15, 20)] private string thai, english;
    private TextMeshProUGUI textMeshPro;
    private Text text;

    private void Awake()
    {
        if (GetComponent<TextMeshProUGUI>() != null) textMeshPro = GetComponent<TextMeshProUGUI>();
        if (GetComponent<Text>() != null) text = GetComponent<Text>();
    }

    private void Update()
    {
        LanguageSelected();
    }

    public void LanguageSelected()
    {
        switch (LocalizationSystem.LocalizationLanguage)
        {
            case Language.Thai:
                if (textMeshPro != null) textMeshPro.text = thai;
                if (text != null) text.text = thai;
                break;
            case Language.English:
                if (textMeshPro != null) textMeshPro.text = english;
                if (text != null) text.text = english;
                break;
        }
    }
}
