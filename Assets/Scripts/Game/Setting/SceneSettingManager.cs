using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class SceneSettingManager : MonoBehaviour
{
    private void Awake()
    {
        audioEffectButton.onClick.AddListener(AudioEffectButton);
        audioBackgroundButton.onClick.AddListener(AudioBackgroundButton);
        profileButton.onClick.AddListener(ProfileButton);
    }

    private void Start()
    {
        settingUi.alpha = 0;
        settingUi.blocksRaycasts = false;
        GetSetting("AudioEffect", audioEffectButton);
        GetSetting("AudioBackground", audioBackgroundButton);
    }


    public void SettingButton()
    {
        settingUi.blocksRaycasts = true;
        DOVirtual.Float(0, 1, 0.3f, v =>
        {
            settingUi.alpha = v;
        }).SetEase(Ease.InOutExpo);
    }

    public void ResumeButton()
    {
        settingUi.blocksRaycasts = false;
        DOVirtual.Float(1, 0, 0.3f, v =>
        {
            settingUi.alpha = v;
        }).SetEase(Ease.InOutExpo);
    }

    private void ProfileButton()
    {
        SceneManager.LoadScene(SceneName.EditInformationPatientScene.ToString());
    }

    public void LogoutButton()
    {

    }

    private void AudioEffectButton()
    {
        SetSetting("AudioEffect", audioEffectButton);
    }

    private void AudioBackgroundButton()
    {
        SetSetting("AudioBackground", audioBackgroundButton);
    }

    public void MoreInfoButton()
    {

    }

    private void GetSetting(string key, Button targetButton)
    {
        if (PlayerPrefs.HasKey(key))
        {
            var value = PlayerPrefs.GetInt(key);
            var text = targetButton.GetComponentInChildren<Text>();
            if (text != null)
            {
                text.text = value == 0 ? "ปิด" : "เปิด";
            }
        }
    }

    private void SetSetting(string key, Button targetButton)
    {
        var text = targetButton.GetComponentInChildren<Text>();
        if (text != null)
        {
            if (text.text == "เปิด")
            {
                text.text = "ปิด";
                PlayerPrefs.SetInt(key, 0);
            }
            else
            {
                text.text = "เปิด";
                PlayerPrefs.SetInt(key, 1);
            }
            PlayerPrefs.Save();
        }
    }
}
