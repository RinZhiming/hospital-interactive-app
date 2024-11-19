using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Firebase.Storage;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardScanner : MonoBehaviour
{
    [SerializeField] private string faceScannerSceneName;
    [SerializeField] private Button captureButton, backButton;
    [SerializeField] private GameObject[] card;
    [SerializeField] private RawImage image;
    private WebCamTexture webcamTexture;
    private DatabaseReference dataRef;
    private FirebaseStorage storage;
    private FirebaseAuth auth;

    private void Awake()
    {
        backButton.onClick.AddListener(BackButton);
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private IEnumerator Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        storage = FirebaseStorage.DefaultInstance;
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
        Debug.Log("Wait for camera");
        
#if UNITY_ANDROID || UNITY_IOS
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        
        yield return new WaitUntil(() => Permission.HasUserAuthorizedPermission(Permission.Camera));
#endif
        
        Debug.Log("Camera Access");
        
        var cam = WebCamTexture.devices;
        if (cam.Length > 0)
        {
            foreach (var c in cam)
            {
                if (!c.isFrontFacing)
                {
                    Debug.Log(c);
                    webcamTexture = new WebCamTexture(c.name,680, 480);
                    break;
                }
            }
        }
        yield return new WaitUntil(() => webcamTexture );
        
        webcamTexture.Play();
        
        Debug.Log("Init Camera Done");
        
        InitCamara();
    }

    private void BackButton()
    {
        auth.SignOut();
    }

    private void OnDestroy()
    {
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
            webcamTexture = null;
        }
    }

    private void InitCamara()
    {
        image.rectTransform.Rotate(new Vector3(0,0,360 - webcamTexture.videoRotationAngle));
        captureButton.onClick.AddListener(CaptureButton);
    }

    private void Update()
    {
        image.texture = webcamTexture;
    }

    private void CaptureButton()
    {
        StartCoroutine(CaptureEvent());
    }

    private IEnumerator CaptureEvent()
    {
        
        foreach (var h in card)
        {
            h.SetActive(false);
        }

        yield return new WaitForEndOfFrame();

        Texture2D sh = ScreenCapture.CaptureScreenshotAsTexture();
        
        var screenshotRef = storage.GetReference($"/CardVerify/{auth.CurrentUser.UserId}.jpg");
        var bytes = sh.EncodeToJPG();
        var uploadTask = screenshotRef.PutBytesAsync(bytes);

        foreach (var h in card)
        {
            h.SetActive(true);
        }
        
        if (uploadTask.Exception != null)
        {
            Debug.Log($"Failed to upload {uploadTask.Exception}");
            yield break;
        }
        
        Debug.Log($"Complete to upload.");
        
        UpdateDatabase();
    }

    private void UpdateDatabase()
    {
        dataRef.Child("users").Child(auth.CurrentUser.UserId).Child("userverify").UpdateChildrenAsync(new Dictionary<string, object>() {{"isCardVerify", true}}).ContinueWithOnMainThread(result =>
        {
            SceneManager.LoadScene(faceScannerSceneName);
        });
    }
}