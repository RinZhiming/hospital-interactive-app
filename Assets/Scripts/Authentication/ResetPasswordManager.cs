using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class ResetPasswordManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TextMeshProUGUI errorText;
    [SerializeField] private UnityEvent completeSendEmailEvent;
    private FirebaseAuth auth;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    public void ResetPassword()
    {
        if (!string.IsNullOrEmpty(emailInput.text))
        {
            auth.SendPasswordResetEmailAsync(emailInput.text).ContinueWithOnMainThread(result =>
            {
                if (result.IsCanceled || result.IsFaulted)
                {
                    errorText.text = "เกิดข้อผิดพลาดบางอย่าง กรุณาลองใหม่อีกครั้ง";
                    if (result.Exception != null) Debug.LogError(result.Exception.Message);
                    return;
                }
                
                if (result.IsCompleted)
                {
                    completeSendEmailEvent?.Invoke();
                }
            });
        }
        else
        {
            errorText.text = "กรุณากรอกข้อมูล";
        }
    }
}
