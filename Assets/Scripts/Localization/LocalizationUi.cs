using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizationUi : MonoBehaviour
{

    [SerializeField] private Button localizationButton;
    [SerializeField] private TextMeshProUGUI text;

    private void Awake()
    {
        localizationButton.onClick.AddListener(SwitchLanguage);
    }

    private void Update()
    {
        text.text = LocalizationSystem.LocalizationLanguage == Language.English ? "EN" : "TH";
    }

    private void SwitchLanguage()
    {
        LocalizationSystem.LocalizationLanguage = LocalizationSystem.LocalizationLanguage == Language.English ? Language.Thai : Language.English;
    }
}
