using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{   
    private FirebaseAuth auth;
    private DatabaseReference dataRef;
    private UserVerify userVerify;

    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;

        auth.StateChanged += AuthStateChanged;
    }

    private void AuthStateChanged(object sender, EventArgs e)
    {
        if (auth.CurrentUser != null)
        {
            LoadManager.Loading(true);
            auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    LoadManager.Loading(false);
                    Debug.Log("ReloadAsync encountered an error: " + result.Exception);
                    return;
                }

                if(result.IsCompleted)
                {
                    if(auth.CurrentUser != null)
                    {
                        EmailVerification();
                    }
                    else
                    {
                        LoadManager.Loading(false);
                        SceneManager.LoadScene(SceneName.LoginScene.ToString());
                    }
                }
            });
        }
        else
        {
            SceneManager.LoadScene(SceneName.LoginScene.ToString());
        }
    }

    private void EmailVerification()
    {
        if (auth.CurrentUser.IsEmailVerified)
        {
            VerifyChecker();
        }
        else
        {
            LoadManager.Loading(false);
            SceneManager.LoadScene(SceneName.EmailVerificationScene.ToString());
        }
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
                        if (result.Exception != null) Debug.LogError(result.Exception.Message);
                        return;
                    }

                    if (result.IsCompleted)
                    {
                        var userverify = JsonUtility.FromJson<UserVerify>(result.Result.GetRawJsonValue());

                        if (!userverify.isCardVerify)
                        {
                            LoadManager.Loading(false);
                            SceneManager.LoadScene(SceneName.CardScanerScene.ToString());
                            return;
                        }

                        if (!userverify.isFaceVerify)
                        {
                            LoadManager.Loading(false);
                            SceneManager.LoadScene(SceneName.FaceScanerScene.ToString());
                            return;
                        }

                        if (!userverify.isPatientVerify)
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

    public void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth = null;
    }
}