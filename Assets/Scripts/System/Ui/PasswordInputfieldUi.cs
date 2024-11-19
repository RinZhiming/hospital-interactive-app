using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PasswordInputfieldUi : MonoBehaviour
{
    [SerializeField] private Button watchPasswordButton;
    [SerializeField] private Sprite eyeOpen, eyeClose;
    [SerializeField] private TMP_InputField passwordInput;

    private void Awake()
    {
        watchPasswordButton.onClick.AddListener(ShowPassword);
    }

    private void ShowPassword()
    {
        passwordInput.contentType = passwordInput.contentType == TMP_InputField.ContentType.Password ? TMP_InputField.ContentType.Standard : TMP_InputField.ContentType.Password;
        watchPasswordButton.image.sprite = passwordInput.contentType == TMP_InputField.ContentType.Password ? eyeOpen : eyeClose;
        passwordInput.ForceLabelUpdate();
    }
}
