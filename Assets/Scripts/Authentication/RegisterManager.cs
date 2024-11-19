using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Firebase;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Database;

public class RegisterManager : MonoBehaviour
{
    [Header("Register Ui")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Dropdown callnameInput;
    [SerializeField] private TMP_InputField firstnameInput;
    [SerializeField] private TMP_InputField lastnameInput;
    [SerializeField] private TMP_InputField birthdayInput;
    [SerializeField] private TMP_InputField ageInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField passwordConfirmInput;
    [Header("Error Register Ui")]
    [SerializeField] private TextMeshProUGUI emailErrorText;
    [SerializeField] private TextMeshProUGUI usernameErrorText;
    [SerializeField] private TextMeshProUGUI firstnameErrorText;
    [SerializeField] private TextMeshProUGUI lastnameErrorText;
    [SerializeField] private TextMeshProUGUI birthdayErrorText;
    [SerializeField] private TextMeshProUGUI ageErrorText;
    [SerializeField] private TextMeshProUGUI passwordErrorText;
    [SerializeField] private TextMeshProUGUI passwordConfirmErrorText;
    [SerializeField] private TextMeshProUGUI agreeTogleErrorText;
    
    [SerializeField] private Toggle agreeTogle;
    [SerializeField] private Button registerButton;

    private bool currectBirthdayFormat, currectUsernameFormat;
    private Dictionary<TMP_InputField, TextMeshProUGUI> errorUiMap = new();
    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference database;

    private void Awake()
    {
        errorUiMap = new()
        {
            {emailInput, emailErrorText},
            {usernameInput, usernameErrorText},
            {firstnameInput, firstnameErrorText},
            {lastnameInput, lastnameErrorText},
            {birthdayInput, birthdayErrorText},
            {ageInput, ageErrorText},
            {passwordInput, passwordErrorText},
            {passwordConfirmInput, passwordConfirmErrorText},
        };
        registerButton.onClick.AddListener(OnRegister);
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        database = FirebaseDatabase.DefaultInstance.RootReference;
        birthdayInput.onValueChanged.AddListener(BirthDayFormat);

        foreach (var map in errorUiMap)
        {
            map.Key.onValueChanged.AddListener(_ =>
            {
                map.Value.text = string.Empty;
            });
        }
        
        agreeTogle.onValueChanged.AddListener(_ =>
        {
            agreeTogleErrorText.text = string.Empty;
        });
    }

    private void UsernameFormat()
    {
        var pattern = @"^[a-z]+$";
        if (Regex.IsMatch(usernameInput.text, pattern))
        {
            currectUsernameFormat = true;
        }
        else
        {
            currectUsernameFormat = false;
            Debug.Log("Incorrect Username");
        }
    }

    private void BirthDayFormat(string v)
    {
        if (v.Length == 3)
        {
            if (v[2] != '/')
            {
                birthdayInput.text = v.Insert(2, "/");
                birthdayInput.MoveToEndOfLine(false, false);
            }
        }
        else if (v.Length == 7)
        {
            if (v[5] != '/')
            {
                birthdayInput.text = v.Insert(5, "/");
                birthdayInput.MoveToEndOfLine(false, false);
            }
        }
        
        else if (v.Length == 10)
        {
            var format = "dd/MM/yyyy";
            if (DateTime.TryParseExact(v, format, null, DateTimeStyles.None, out DateTime _))
            {
                currectBirthdayFormat = true;
            }
            else
            {
                currectBirthdayFormat = false;
                birthdayErrorText.text = "Incorrect Date Format";
            }
        }
        else
        {
            currectBirthdayFormat = false;
        }
    }

    private void OnRegister()
    {
        LoadManager.Loading(true);
        ClearError();
        var error = ValidateCheckInput();
        
        if (error.Count > 0)
        {
            DisplayError(error);
        }
        else
        {
            Register();
        }
    }
    
    private Dictionary<TextMeshProUGUI, string> ValidateCheckInput()
    {
        var handleError = new Dictionary<TextMeshProUGUI, string>();
        
        UsernameFormat();
        
        foreach (var map in errorUiMap)
        {
            if (string.IsNullOrEmpty(map.Key.text)) handleError.TryAdd(map.Value, "กรุณากรอกข้อมูล");
        }
        
        if (!currectBirthdayFormat)
            handleError.TryAdd(birthdayErrorText, "วันเดือนปีเกิดไม่ถูกต้อง");
        
        if (!currectUsernameFormat)
            handleError.TryAdd(usernameErrorText, "username ไม่ตรงเงื่อนไข");
        
        if(passwordInput.text.Length < 8)
            handleError.TryAdd(passwordErrorText, "รหัสผ่านไม่ตรงตามเงื่อนไข");
        
        if(passwordInput.text != passwordConfirmInput.text) 
            handleError.TryAdd(passwordConfirmErrorText, "รหัสผ่านไม่ตรงกัน");
        
        if(!agreeTogle.isOn)
            handleError.TryAdd(agreeTogleErrorText, "กรุณายอมรับก่อนลงทะเบียน");
        
        return handleError;
    }
    
    private void DisplayError(Dictionary<TextMeshProUGUI, string> error)
    {
        foreach (var e in error)
        {
            e.Key.text = e.Value;
        }
        
        LoadManager.Loading(false);
    }
    
    private void ClearError()
    {
        foreach (var map in errorUiMap)
        {
            map.Value.text = string.Empty;
        }

        agreeTogleErrorText.text = string.Empty;
    }
    
    private void Register()
    {
        auth.CreateUserWithEmailAndPasswordAsync(emailInput.text, passwordInput.text).ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                LoadManager.Loading(false);
                if (result.Exception != null)
                {
                    Debug.LogError(result.Exception.InnerExceptions);
                    foreach (var exception in result.Exception.Flatten().InnerExceptions)
                    {
                        if (exception is FirebaseException ex)
                        {
                            switch ((AuthError)ex.ErrorCode)
                            {
                                case AuthError.InvalidEmail:
                                    emailErrorText.text = "ไม่สามารถใช้ email นี้ได้";
                                    break;
                                case AuthError.EmailAlreadyInUse:
                                    emailErrorText.text = "Email นี้ถูกลงทะเบียนแล้ว";
                                    break;
                                default:
                                    emailErrorText.text = "Email ไม่ถูกต้อง";
                                    break;
                            }
                        }
                    }
                }
                return;
            }
            if (result.IsCompleted)
            {
                user = auth.CurrentUser;
                
                UpdateData();
            }
        });
    }
    
    private void UpdateData()
    {
        UserData newUser = new()
        {
            username = usernameInput.text,
            callname = callnameInput.value,
            firstname = firstnameInput.text,
            lastname = lastnameInput.text,
            birthday = birthdayInput.text,
            age = int.Parse(ageInput.text),
            iconProfile = -1,
        };

        string userJson = JsonUtility.ToJson(newUser);

        database.Child("users").Child(user.UserId).Child("userdata").SetRawJsonValueAsync(userJson).ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                LoadManager.Loading(false);
                if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                return;
            }
            if (result.IsCompleted)
            {
                Debug.Log("User added.");
                UserDevice();
            }
        });
    }
    
    private void UserDevice()
    {
        UserDevice newdevice = new()
        {
            device = "none",
        };

        string deviceJson = JsonUtility.ToJson(newdevice);

        database.Child("users").Child(user.UserId).Child("device").SetRawJsonValueAsync(deviceJson).ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                LoadManager.Loading(false);
                if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                return;
            }
            if (result.IsCompleted)
            {
                Debug.Log("Device added.");
                UserVerify();
            }
        });
    }
    
    private void UserVerify()
    {
        UserVerify newverify = new()
        {
            isCardVerify = false,
            isFaceVerify = false,
            isPatientVerify = false,
        };

        string verifyJson = JsonUtility.ToJson(newverify);

        database.Child("users").Child(user.UserId).Child("userverify").SetRawJsonValueAsync(verifyJson).ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                LoadManager.Loading(false);
                if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                return;
            }
            if (result.IsCompleted)
            {
                Debug.Log("Verify added.");
                LoadManager.Loading(false);
                EmailVerification();
            }
        });
    }
    
    private void EmailVerification()
    {
        SceneManager.LoadScene(SceneName.EmailVerificationScene.ToString());
    }
}