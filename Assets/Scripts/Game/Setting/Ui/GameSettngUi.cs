using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class GameSettingManager
{
    [SerializeField] private CanvasGroup settingUi;
    [SerializeField] private Button audioBackgroundButton, audioEffectButton, resumeButton, settingButton, profileButton;
    [SerializeField] private GameObject[] settingUiObjects;
}