using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationDropdownEvent : MonoBehaviour
{
    [SerializeField] private List<string> thai = new List<string>();
    [SerializeField] private List<string> english = new List<string>();
    private TMP_Dropdown dropdownTMP;

    private void Awake()
    {
        dropdownTMP = GetComponent<TMP_Dropdown>();
    }

    private void Start()
    {
        LanguageSelected();
    }

    public void LanguageSelected()
    {
        switch (LocalizationSystem.LocalizationLanguage)
        {
            case Language.Thai:
                if (dropdownTMP != null) dropdownTMP.ClearOptions();
                dropdownTMP?.AddOptions(thai);
                break;
            case Language.English:
                if (dropdownTMP != null) dropdownTMP.ClearOptions();
                dropdownTMP?.AddOptions(english);
                break;
        }
    }
}
