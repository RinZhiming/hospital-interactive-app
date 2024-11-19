using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using TMPro;
using UnityEngine;

public partial class ProfileManager : MonoBehaviour
{
    [SerializeField] private LocalizationAddress localizationAddress;
    private Dictionary<TMP_InputField, TextMeshProUGUI> errorMapUi = new();
    private Dictionary<TMP_InputField, TextMeshProUGUI> optionalPatientErrorMapUi = new();
    private Dictionary<TMP_InputField, TextMeshProUGUI> optionalOtherErrorMapUi = new();
    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    private FirebaseUser user;
    
    private void Awake()
    {
        errorMapUi = new()
        {
            {cardIdentityInput, cardIdentityErrorText},
            {nationalityInput, nationalityErrorText},
            {jobInput, jobErrorText},
            
            {houseNumberInput, houseNumberErrorText},
            {villageNumberInput, villageNumberErrorText},
            {villageInput, villageErrorText},
            {alleyInput, alleyErrorText},
            {roadInput, roadErrorText},
            
            {mobilePhoneInput, mobilePhoneErrorText},
            {emailInput, emailErrorText},
            
            {name1Input, name1ErrorText},
            {relationship1Input, relationship1ErrorText},
            {phone1Input, phone1ErrorText},
        };
        
        optionalPatientErrorMapUi = new()
        {
            {name2Input, name2ErrorText},
            {relationship2Input, relationship2ErrorText},
            {phone2Input, phone2ErrorText},
        };
        
        optionalOtherErrorMapUi = new()
        {
            {otherNameInput, otherNameErrorText},
            {otherPhoneNumberInput, otherPhoneNumberErrorText},
        };
                
        foreach (var error in errorMapUi)
        {
            error.Key.onValueChanged.AddListener(_ =>
            {
                error.Value.text = string.Empty;
            });
        }
        
        foreach (var error in optionalPatientErrorMapUi)
        {
            error.Key.onValueChanged.AddListener(_ =>
            {
                error.Value.text = string.Empty;
            });
        }
        
        foreach (var error in optionalOtherErrorMapUi)
        {
            error.Key.onValueChanged.AddListener(_ =>
            {
                error.Value.text = string.Empty;
            });
        }

        allergyMedicineInput.onValueChanged.AddListener(_ =>
        {
            allergyMedicineErrorText.text = string.Empty;
        });
        
        allergyFoodInput.onValueChanged.AddListener(_ =>
        {
            allergyFoodErrorText.text = string.Empty;
        });
        
        dataButton.onClick.AddListener(UpdateData);
        backButton.onClick.AddListener(BackButton);

        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        user = auth.CurrentUser;

        StartCoroutine(WaitForInit());
    }

    private IEnumerator WaitForInit()
    {
        LoadManager.Loading(true);
        yield return new WaitUntil(() => localizationAddress.IsInitDone);
        LoadGeneralData();
    }

    private void Update()
    {
        medicineGroup.gameObject.SetActive(allergyMedicineToggle.isOn);
        foodGroup.gameObject.SetActive(allergyFoodToggle.isOn);
        patient2Group.gameObject.SetActive(!sameToggle.isOn);
        otherNameInput.gameObject.SetActive(otherToggle.isOn);
        otherPhoneNumberInput.gameObject.SetActive(otherToggle.isOn);
    }

    private void LoadGeneralData()
    {
        Debug.Log("Is Load Data...");
        dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("generaldata").GetValueAsync()
            .ContinueWithOnMainThread(result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    LoadManager.Loading(false);
                    if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                    return;
                }

                if (result.IsCompleted)
                {
                    var data = JsonUtility.FromJson<GeneralData>(result.Result.GetRawJsonValue());
                    SetGeneralDataUi(data);
                }
            });
    }

    private void SetGeneralDataUi(GeneralData data)
    {
        cardIdentityInput.text = data.cardIdentity;
        nationalityInput.text = data.nationality;
        jobInput.text = data.job;
        sexDropdown.value = data.sex;
        religionDropdown.value = data.religion;
        stateDropdown.value = data.state;
        housePhoneInput.text = data.housePhone;
        mobilePhoneInput.text = data.mobilePhone;
        emailInput.text = data.email;
        
        LoadAddressData();
    }

    private void LoadAddressData()
    {
        dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("adrressdata").GetValueAsync()
            .ContinueWithOnMainThread(result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    LoadManager.Loading(false);
                    if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                    return;
                }

                if (result.IsCompleted)
                {
                    var data = JsonUtility.FromJson<AddressData>(result.Result.GetRawJsonValue());
                    SetAddressDataUi(data);
                }
            });
    }
    
    private void SetAddressDataUi(AddressData data)
    {
        houseNumberInput.text = data.houseNumber;
        villageNumberInput.text = data.villageNumber;
        villageInput.text = data.village;
        alleyInput.text = data.alley;
        roadInput.text = data.road;
        provinceNumberDropdown.value = data.provinceNumber;
        districtDropdown.value = data.district;
        subDistrictDropdown.value = data.subDistrict;
        postalCodeDropdown.text = data.postalCode;
        
        LoadPatientData();
    }
    
    private void LoadPatientData()
    {
        dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("patientdata").GetValueAsync()
            .ContinueWithOnMainThread(result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    LoadManager.Loading(false);
                    if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                    return;
                }

                if (result.IsCompleted)
                {
                    var data = JsonUtility.FromJson<PatientData>(result.Result.GetRawJsonValue());
                    SetPatientDataUi(data);
                }
            });
    }
    
    private void SetPatientDataUi(PatientData data)
    {
        name1Input.text = data.name1;
        phone1Input.text = data.phone1;
        relationship1Input.text = data.relationship1;
        
        if (data.name1 != data.name2 &&
            data.relationship1 != data.relationship2 &&
            data.phone1 != data.phone2)
        {
            name2Input.text = data.name2;
            phone2Input.text = data.phone2;
            relationship2Input.text = data.relationship2;
            sameToggle.isOn = false;
        }
        else
        {
            sameToggle.isOn = true;
        }
        
        LoadMedicalData();
    }
    
    private void LoadMedicalData()
    {
        dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("medicaldata").GetValueAsync()
            .ContinueWithOnMainThread(result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    LoadManager.Loading(false);
                    if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                    return;
                }

                if (result.IsCompleted)
                {
                    var data = JsonUtility.FromJson<MedicalData>(result.Result.GetRawJsonValue());
                    SetMedicalDataUi(data);
                }
            });
    }
    
    private void SetMedicalDataUi(MedicalData data)
    {
        unknowToggle.isOn = data.unknowAllergy;
        noAllergyToggle.isOn = data.noAllergy;
        allergyMedicineToggle.isOn = data.allergyMedicine;
        allergyFoodToggle.isOn = data.allergyFood;
        patientToggle.isOn = data.patient;
        relativeToggle.isOn = data.relative;
        otherToggle.isOn = data.other;

        if (data.allergyMedicine)
        {
            allergyMedicineInput.text = data.allergyMedicineInfo;
        }

        if (data.allergyFood)
        {
            allergyFoodInput.text = data.allergyFoodInfo;
        }

        if (data.other)
        {
            otherNameInput.text = data.otherNameInfo;
            otherPhoneNumberInput.text = data.otherPhoneInfo;
        }
        
        LoadManager.Loading(false);
    }

    private void UpdateData()
    {
        LoadManager.Loading(true);
        ClearError();
        var error = ValidateInput();

        if (error.Count > 0)
        {
            firstPage.SetActive(true);
            lastPage.SetActive(false);
            DisplayError(error);
        }
        else
        {
            SetGeneralData();
        }
    }
    
    private Dictionary<TextMeshProUGUI, string> ValidateInput()
    {
        var handleError = new Dictionary<TextMeshProUGUI, string>();
        
        foreach (var error in errorMapUi)
        {
            if (string.IsNullOrEmpty(error.Key.text)) handleError.TryAdd(error.Value, "กรุณากรอกข้อมูล");
        }

        if (!sameToggle.isOn)
        {
            foreach (var error in optionalPatientErrorMapUi)
            {
                if (string.IsNullOrEmpty(error.Key.text)) handleError.TryAdd(error.Value, "กรุณากรอกข้อมูล");
            }
        }

        if (otherToggle.isOn)
        {
            foreach (var error in optionalOtherErrorMapUi)
            {
                if (string.IsNullOrEmpty(error.Key.text)) handleError.TryAdd(error.Value, "กรุณากรอกข้อมูล");
            }
        }
        
        if (allergyMedicineToggle.isOn)
        {
            if (string.IsNullOrEmpty(allergyMedicineInput.text))
                handleError.TryAdd(allergyMedicineErrorText, "กรุณากรอกข้อมูล");
        }
        
        if (allergyFoodToggle.isOn)
        {
            if (string.IsNullOrEmpty(allergyFoodInput.text))
                handleError.TryAdd(allergyFoodErrorText, "กรุณากรอกข้อมูล");
        }

        if (cardIdentityInput.text.Length != 13)
            handleError.TryAdd(cardIdentityErrorText, "ข้อมูลไม่ถูกต้อง");
        
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
                
        foreach (var error in errorMapUi)
        {
            error.Value.text = string.Empty;
        }
        
        foreach (var error in optionalPatientErrorMapUi)
        {
            error.Value.text = string.Empty;
        }
        
        foreach (var error in optionalOtherErrorMapUi)
        {
            error.Value.text = string.Empty;
        }

        allergyMedicineErrorText.text = string.Empty;
        allergyFoodErrorText.text = string.Empty;
    }

    private void BackButton()
    {
        GameManager.StartShared("Main", 3);
    }

    private void SetGeneralData()
    {
        GeneralData general = new GeneralData()
        {
            cardIdentity = cardIdentityInput.text,
            sex = sexDropdown.value,
            nationality = nationalityInput.text,
            religion = religionDropdown.value,
            state = stateDropdown.value,
            job = jobInput.text,
            housePhone = housePhoneInput.text,
            mobilePhone = mobilePhoneInput.text,
            email = emailInput.text
        };

        string generaljson = JsonUtility.ToJson(general);

        dbRef.Child("users").Child(user.UserId).Child("generaldata").SetRawJsonValueAsync(generaljson)
            .ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        LoadManager.Loading(false);
                        if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                        return;
                    }
                    
                    if (result.IsCompleted) SetAddressData();
                });
    }

    private void SetAddressData()
    {
        AddressData adrress = new AddressData()
        {
            houseNumber = houseNumberInput.text,
            villageNumber = villageNumberInput.text,
            village = villageInput.text,
            alley = alleyInput.text,
            road = roadInput.text,
            provinceNumber = provinceNumberDropdown.value,
            district = districtDropdown.value,
            subDistrict = subDistrictDropdown.value,
            postalCode = postalCodeDropdown.text,
        };

        string adrressjson = JsonUtility.ToJson(adrress);

        dbRef.Child("users").Child(user.UserId).Child("adrressdata").SetRawJsonValueAsync(adrressjson)
            .ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        LoadManager.Loading(false);
                        if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                        return;
                    }
                    
                    if (result.IsCompleted)
                        SetPatientData();
                });
    }

    private void SetPatientData()
    {
        if(sameToggle.isOn)
        {
            name2Input.text = name1Input.text;
            relationship2Input.text = relationship1Input.text;
            phone2Input.text = phone1Input.text;
        }

        PatientData patient = new PatientData()
        {
            name1 = name1Input.text,
            relationship1 = relationship1Input.text,
            phone1 = phone1Input.text,
            name2 = name2Input.text,
            relationship2 = relationship2Input.text,
            phone2 = phone2Input.text,
        };

        string patientjson = JsonUtility.ToJson(patient);

        dbRef.Child("users").Child(user.UserId).Child("patientdata").SetRawJsonValueAsync(patientjson)
            .ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        LoadManager.Loading(false);
                        if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                        return;
                    }
                    
                    if (result.IsCompleted)
                        SetMedicalData();
                });
    }

    private void SetMedicalData()
    {
        MedicalData medical = new MedicalData()
        {
            unknowAllergy = unknowToggle.isOn,
            noAllergy = noAllergyToggle.isOn,
            allergyMedicine = allergyMedicineToggle.isOn,
            allergyFood = allergyFoodToggle.isOn,
            patient = patientToggle.isOn,
            relative = relativeToggle.isOn,
            other = otherToggle.isOn,
            allergyMedicineInfo = allergyMedicineInput.text,
            allergyFoodInfo = allergyFoodInput.text,
            otherNameInfo = otherNameInput.text,
            otherPhoneInfo = otherPhoneNumberInput.text,
        };
        string medicaljson = JsonUtility.ToJson(medical);

        dbRef.Child("users").Child(user.UserId).Child("medicaldata").SetRawJsonValueAsync(medicaljson)
            .ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        LoadManager.Loading(false);
                        if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                        return;
                    }

                    if (result.IsCompleted)
                    {
                        LoadManager.Loading(false);
                        GameManager.StartShared("Main", 3);
                    }
                });
    }
}
