using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

public partial class GameSettingManager : MonoBehaviour
{
    private void Awake()
    {
        settingButton.onClick.AddListener(SettingButton);
        resumeButton.onClick.AddListener(ResumeButton);
        audioEffectButton.onClick.AddListener(AudioEffectButton);
        audioBackgroundButton.onClick.AddListener(AudioBackgroundButton);
        profileButton.onClick.AddListener(ProfileButton);

        settingUiObjects.SetActive(false);
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        settingUi.alpha = 0;
        settingUi.blocksRaycasts = false;
        PlayerManager.IsPause = false;
        GetSetting("AudioEffect", audioEffectButton);
        GetSetting("AudioBackground", audioBackgroundButton);
    }

    private void Update()
    {
        settingButton.gameObject.SetActive(!HealthInformationManager.IsActive);

        settingUiObjects.SetActive(SceneManager.GetActiveScene().buildIndex == 3);
    }
    
    private void ProfileButton()
    {
        GameManager.Launcher.OnLeftRoom();
        SceneManager.LoadScene(SceneName.EditInformationPatientScene.ToString());
    }

    private void AudioBackgroundButton()
    {
        SetSetting("AudioBackground", audioBackgroundButton);
    }

    private void AudioEffectButton()
    {
        SetSetting("AudioEffect", audioEffectButton);
    }

    private void SettingButton()
    {
        PlayerManager.IsPause = true;
        settingUi.blocksRaycasts = true;
        DOVirtual.Float(0, 1, 0.3f, v =>
        {
            settingUi.alpha = v;
        }).SetEase(Ease.InOutExpo);
    }

    private void ResumeButton()
    {
        PlayerManager.IsPause = false;
        settingUi.blocksRaycasts = false;
        DOVirtual.Float(1, 0, 0.3f, v =>
        {
            settingUi.alpha = v;
        }).SetEase(Ease.InOutExpo);
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