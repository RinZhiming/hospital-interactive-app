using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;
using UnityEngine.UI;

public partial class CharacterSelectManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    private void Awake()
    {
        CharacterIndex = 0;
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        
        backButton.onClick.AddListener(OnBackButtonClicked);
        nextButton.onClick.AddListener(OnNextButtonClicked);
        selectButton.onClick.AddListener(OnSelectButtonClicked);

        contain.localPosition = new Vector3(-200f, contain.localPosition.y, contain.localPosition.z);

        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    private void OnSelectButtonClicked()
    {
        dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("userdata").Child("iconProfile").SetValueAsync(CharacterIndex)
            .ContinueWithOnMainThread(result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        Debug.LogError(result.Exception.Message);
                        return;
                    }

                    if (result.IsCompleted)
                    {
                        GameManager.StartShared("Main",3);
                    }
                });
    }

    private void OnNextButtonClicked()
    {
        if (contain.localPosition.x <= -2700f) return;
        if (!onSlide)
        { 
            StartCoroutine(ScrollDelay(contain.localPosition.x, contain.localPosition.x - 500f));
            CharacterIndex++;
        }
    }

    private void OnBackButtonClicked()
    {
        if (contain.localPosition.x > -300f) return;
        if (!onSlide)
        { 
            StartCoroutine(ScrollDelay(contain.localPosition.x, contain.localPosition.x + 500f));
            CharacterIndex--;
        }
    }

    private IEnumerator ScrollDelay(float start, float end)
    {
        onSlide = true;
        DOVirtual.Float(start, end, scrollSpeed, f =>
        {
            contain.localPosition = new Vector3(f, contain.localPosition.y, contain.localPosition.z);
        });
        yield return new WaitForSeconds(scrollSpeed + 0.1f);
        onSlide = false;
    }
}
