using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReSendEmailVerificationManager : MonoBehaviour
{
    [SerializeField] private Button resendButton;

    private FirebaseAuth auth;
    private FirebaseUser user;

    private void Awake()
    {
        resendButton.onClick.AddListener(EmailVerification);
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;

        EmailVerification();
    }

    private void EmailVerification()
    {
        user.SendEmailVerificationAsync().ContinueWithOnMainThread(result =>
        {
        });
    }


    private void EmailVerificationSuccess()
    {
        auth.SignOut();
        SceneManager.LoadScene(SceneName.LoginScene.ToString());
    }
}
