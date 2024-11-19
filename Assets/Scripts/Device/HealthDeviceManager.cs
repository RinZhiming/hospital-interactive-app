using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json.Linq;
using UnityEngine;

public partial class HealthDeviceManager : MonoBehaviour
{
    public static event Action<string> OnAddDevice;
    public static event Action<string> OnRemoveDevice;
    private readonly object deviceLockObj = new();
    private string deviceId;
    private DatabaseReference dbRef;
    private FirebaseAuth auth;
    
    private void Awake()
    {
        connectButton.onClick.AddListener(ConnectButton);
        connectorButton.onClick.AddListener(ConnectorButton);
        deviceInput.onValueChanged.AddListener(InputValidate);
        backButton.onClick.AddListener(BackButton);
        
        connectButton.interactable = false;
    }


    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        deviceCanvasGroup.SetActive(false);
        
        errorText.text = string.Empty;
        GetDeviceId();
    }
    
    private void InputValidate(string s)
    {
        connectButton.interactable = !string.IsNullOrEmpty(s);
        errorText.text = string.Empty;
    }

    private void GetDeviceId()
    {
        dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("device").GetValueAsync().ContinueWithOnMainThread(
            result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    Debug.LogError(result.Exception?.Message);
                    errorText.text = result.Exception?.Message;
                    return;
                }

                if (result.IsCompleted)
                {
                    var dataraw = result.Result.GetRawJsonValue();
                    var data = JObject.Parse(dataraw);
                    var id = data["device"]?.ToString();

                    if (!string.IsNullOrEmpty(id))
                    {
                        DeviceConnector(id);
                    }
                }
            });
    }

    private void BackButton()
    {
        deviceCanvasGroup.SetActive(false);
    }

    private void ConnectorButton()
    {
        deviceCanvasGroup.SetActive(true);
        errorText.text = string.Empty;
        GetDeviceId();
    }
    
    private void ConnectButton()
    {
        if (!string.IsNullOrEmpty(deviceInput.text))
        {
            CheckDeviceId();
        }
    }

    private void CheckDeviceId()
    {
        dbRef.Child("devices").Child(deviceInput.text).GetValueAsync().ContinueWithOnMainThread(
            result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    Debug.LogError(result.Exception?.Message);
                    errorText.text = result.Exception?.Message;
                    return;
                }

                if (result.IsCompleted)
                {
                    var data = result.Result.GetRawJsonValue();
                    if (!string.IsNullOrEmpty(data))
                    {
                        SetDeviceId();
                    }
                    else
                    {
                        errorText.text = "No device found.";
                    }
                }
            });
    }

    private void SetDeviceId()
    {
        lock (deviceLockObj)
        {
            var newDeviceId = new UserDevice { device = deviceInput.text };
            var newDeviceIdData = JsonUtility.ToJson(newDeviceId);
            dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("device").SetRawJsonValueAsync(newDeviceIdData)
                .ContinueWithOnMainThread(
                    result =>
                    {
                        if (result.IsFaulted || result.IsCanceled)
                        {
                            Debug.LogError(result.Exception?.Message);
                            errorText.text = result.Exception?.Message;
                            return;
                        }

                        if (result.IsCompleted)
                        {
                            OnRemoveDevice?.Invoke(deviceId);
                            OnAddDevice?.Invoke(deviceInput.text);
                            
                            deviceId = deviceInput.text;
                            deviceCanvasGroup.SetActive(false);
                        }
                    });
        }
    }

    private void DeviceConnector(string id)
    {
        OnAddDevice?.Invoke(id);
        deviceInput.text = id;
        deviceId = id;
    }

    public void OnDestroy()
    {
        if (!string.IsNullOrEmpty(deviceId)) OnRemoveDevice?.Invoke(deviceId);
    }
}
