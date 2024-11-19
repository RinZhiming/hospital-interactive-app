using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class HealthDeviceManager
{
    [SerializeField] private InputField deviceInput;
    [SerializeField] private CanvasGroup deviceCanvasGroup;
    [SerializeField] private Button backButton, connectButton, connectorButton;
    [SerializeField] private Text errorText;
}
