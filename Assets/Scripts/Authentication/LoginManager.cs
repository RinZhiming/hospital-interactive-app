using Firebase.Auth;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton, googleLoginButton, registerButton, forgetPasswordButton;
    [SerializeField] private TextMeshProUGUI errorText;

    private FirebaseAuth auth;
    private DatabaseReference dataRef;
    private FirebaseUser user;

    private void Awake()
    {
        loginButton.onClick.AddListener(Login);
        registerButton.onClick.AddListener(ToRegister);
        forgetPasswordButton.onClick.AddListener(ForgetPassword);

        emailInput.onValueChanged.AddListener(s =>
        {
            errorText.text = string.Empty;
        });
        
        passwordInput.onValueChanged.AddListener(s =>
        {
            errorText.text = string.Empty;
        });
        
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
    }
    
    private void Login()
    {
        LoadManager.Loading(true);

        if (string.IsNullOrEmpty(emailInput.text) || string.IsNullOrEmpty(passwordInput.text))
        {
            LoadManager.Loading(false);
            errorText.text = "ไม่สามารถเข้าสู่ระบบได้";
        }
        else
        {
            LogInProcess();
        }
    }

    private void LogInProcess()
    {
        auth.SignInWithEmailAndPasswordAsync(emailInput.text, passwordInput.text).ContinueWithOnMainThread(result =>
        {
            if (result.IsCanceled || result.IsFaulted)
            {
                errorText.text = "ไม่สามารถเข้าสู่ระบบได้";
                LoadManager.Loading(false);
                Debug.LogError(result.Exception.Message);
                return;
            }
            if (result.IsCompleted)
            {
                Reload();
            }
        });
    }

    private void Reload()
    {
        auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(result =>
        {
            if (result.IsCanceled)
            {
                LoadManager.Loading(false);
                Debug.Log("ReloadAsync was canceled.");
                return;
            }
            if (result.IsFaulted)
            {
                LoadManager.Loading(false);
                Debug.Log("ReloadAsync encountered an error: " + result.Exception);
                return;
            }

            if (result.IsCompleted)
            {
                if (auth.CurrentUser != null)
                {
                    user = auth.CurrentUser;

                    if (user.IsEmailVerified)
                    {
                        VerifyChecker();
                    }
                    else
                    {
                        LoadManager.Loading(false);
                        SceneManager.LoadScene(SceneName.EmailVerificationScene.ToString());
                    }
                }
            }
        });
    }

    private void VerifyChecker()
    {
        dataRef.Child("users").Child(auth.CurrentUser.UserId).Child("userverify").GetValueAsync()
            .ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        LoadManager.Loading(false);
                        Debug.LogError(result.Exception.Message);
                        return;
                    }

                    if (result.IsCompleted)
                    {
                        var userVerify = JsonUtility.FromJson<UserVerify>(result.Result.GetRawJsonValue());

                        if (!userVerify.isCardVerify)
                        {
                            LoadManager.Loading(false);
                            SceneManager.LoadScene(SceneName.CardScanerScene.ToString());
                            return;
                        }

                        if (!userVerify.isFaceVerify)
                        {
                            LoadManager.Loading(false);
                            SceneManager.LoadScene(SceneName.FaceScanerScene.ToString());
                            return;
                        }

                        if (!userVerify.isPatientVerify)
                        {
                            LoadManager.Loading(false);
                            SceneManager.LoadScene(SceneName.InformationPatientScene.ToString());
                            return;
                        }

                        IconProfileCheck();
                    }
                });
    }

    private void IconProfileCheck()
    {
        dataRef.Child("users").Child(auth.CurrentUser.UserId).Child("userdata").GetValueAsync()
            .ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        LoadManager.Loading(false);
                        Debug.LogError(result.Exception.Message);
                        return;
                    }

                    if (result.IsCompleted)
                    {
                        var user = JsonUtility.FromJson<UserData>(result.Result.GetRawJsonValue());

                        if (user.iconProfile > -1)
                        {
                            LoadManager.Loading(false);
                            CharacterSelectManager.CharacterIndex = user.iconProfile;
                            GameManager.StartShared("Main",3);
                        }
                        else
                        {
                            LoadManager.Loading(false);
                            SceneManager.LoadScene(SceneName.CharacterSelectScene.ToString());
                        }
                    }
                });
    }

    private void ToRegister()
    {
        SceneManager.LoadScene(SceneName.RegisterScene.ToString());
    }

    private void ForgetPassword()
    {
        SceneManager.LoadScene(SceneName.ForgotPasswordScene.ToString());
    }
}
