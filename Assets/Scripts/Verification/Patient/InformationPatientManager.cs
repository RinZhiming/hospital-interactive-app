using System;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InformationPatientManager : MonoBehaviour
{
    [Header("Ui page 1")]
    [SerializeField] private TMP_InputField cardIdentityInput;
    [SerializeField] private TMP_Dropdown sexDropdown;
    [SerializeField] private TMP_InputField nationalityInput;
    [SerializeField] private TMP_Dropdown religionDropdown;
    [SerializeField] private TMP_Dropdown stateDropdown;
    [SerializeField] private TMP_InputField jobInput;
    [Header("Error Ui page 1")]
    [SerializeField] private TextMeshProUGUI cardIdentityErrorText;
    [SerializeField] private TextMeshProUGUI sexErrorText;
    [SerializeField] private TextMeshProUGUI nationalityErrorText;
    [SerializeField] private TextMeshProUGUI religionErrorText;
    [SerializeField] private TextMeshProUGUI stateErrorText;
    [SerializeField] private TextMeshProUGUI jobErrorText;
    
    [Header("Ui page 2")]
    [SerializeField] private TMP_InputField houseNumberInput;
    [SerializeField] private TMP_InputField villageNumberInput;
    [SerializeField] private TMP_InputField villageInput;
    [SerializeField] private TMP_InputField alleyInput;
    [SerializeField] private TMP_InputField roadInput;
    [SerializeField] private TMP_Dropdown provinceNumberDropdown;
    [SerializeField] private TMP_Dropdown districtDropdown;
    [SerializeField] private TMP_Dropdown subDistrictDropdown;
    [SerializeField] private TMP_InputField postalCodeDropdown;
    [Header("Error Ui page 2")]
    [SerializeField] private TextMeshProUGUI houseNumberErrorText;
    [SerializeField] private TextMeshProUGUI villageNumberErrorText;
    [SerializeField] private TextMeshProUGUI villageErrorText;
    [SerializeField] private TextMeshProUGUI alleyErrorText;
    [SerializeField] private TextMeshProUGUI roadErrorText;
    [SerializeField] private TextMeshProUGUI provinceNumberErrorText;
    [SerializeField] private TextMeshProUGUI districtErrorText;
    [SerializeField] private TextMeshProUGUI subDistrictErrorText;
    [SerializeField] private TextMeshProUGUI postalCodeErrorText;
    
    [Header("Ui page 3")]
    [SerializeField] private TMP_InputField housePhoneInput;
    [SerializeField] private TMP_InputField mobilePhoneInput;
    [SerializeField] private TMP_InputField emailInput;
    [Header("Error Ui page 3")]
    [SerializeField] private TextMeshProUGUI housePhoneErrorText;
    [SerializeField] private TextMeshProUGUI mobilePhoneErrorText;
    [SerializeField] private TextMeshProUGUI emailErrorText;

    [Header("Ui page 4")]
    [SerializeField] private TMP_InputField name1Input;
    [SerializeField] private TMP_InputField relationship1Input;
    [SerializeField] private TMP_InputField phone1Input;
    [SerializeField] private TMP_InputField name2Input;
    [SerializeField] private TMP_InputField relationship2Input;
    [SerializeField] private TMP_InputField phone2Input;
    [SerializeField] private Toggle sameToggle;
    [Header("Error Ui page 4")]
    [SerializeField] private TextMeshProUGUI name1ErrorText;
    [SerializeField] private TextMeshProUGUI relationship1ErrorText;
    [SerializeField] private TextMeshProUGUI phone1ErrorText;
    [SerializeField] private TextMeshProUGUI name2ErrorText;
    [SerializeField] private TextMeshProUGUI relationship2ErrorText;
    [SerializeField] private TextMeshProUGUI phone2ErrorText;

    [Header("Ui page 5")]
    [SerializeField] private Toggle unknowToggle;
    [SerializeField] private Toggle noAllergyToggle;
    [SerializeField] private Toggle allergyMedicineToggle;
    [SerializeField] private Toggle allergyFoodToggle;
    [SerializeField] private Toggle patientToggle;
    [SerializeField] private Toggle relativeToggle;
    [SerializeField] private Toggle otherToggle;
    [SerializeField] private TMP_InputField allergyMedicineInput;
    [SerializeField] private TMP_InputField allergyFoodInput;
    [SerializeField] private TMP_InputField otherNameInput;
    [SerializeField] private TMP_InputField otherPhoneNumberInput;
    [Header("Error Ui page 5")]
    [SerializeField] private TextMeshProUGUI allergyMedicineErrorText;
    [SerializeField] private TextMeshProUGUI allergyFoodErrorText;
    [SerializeField] private TextMeshProUGUI otherNameErrorText;
    [SerializeField] private TextMeshProUGUI otherPhoneNumberErrorText;
    [SerializeField] private GameObject lastPage, firstPage;

    [SerializeField] private GameObject medicineGroup, foodGroup, patient2Group;

    [SerializeField] private Button dataButton, skipButton;
    
    private Dictionary<TMP_InputField, TextMeshProUGUI> errorMapUi = new();
    private Dictionary<TMP_InputField, TextMeshProUGUI> optionalPatientErrorMapUi = new();
    private Dictionary<TMP_InputField, TextMeshProUGUI> optionalOtherErrorMapUi = new();

    private FirebaseAuth auth;
    private FirebaseUser user;
    private DatabaseReference dataRef;

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
        skipButton.onClick.AddListener(Skip);

        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        user = auth.CurrentUser;
        dataRef = FirebaseDatabase.DefaultInstance.RootReference;
    }

    private void Update()
    {
        medicineGroup.gameObject.SetActive(allergyMedicineToggle.isOn);
        foodGroup.gameObject.SetActive(allergyFoodToggle.isOn);
        patient2Group.gameObject.SetActive(!sameToggle.isOn);
        otherNameInput.gameObject.SetActive(otherToggle.isOn);
        otherPhoneNumberInput.gameObject.SetActive(otherToggle.isOn);
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

    private void Skip()
    {
        SceneManager.LoadScene("CharacterSelectScene");
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

        var generaljson = JsonUtility.ToJson(general);

        dataRef.Child("users").Child(user.UserId).Child("generaldata").SetRawJsonValueAsync(generaljson)
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
        var address = new AddressData()
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

        var adrressjson = JsonUtility.ToJson(address);

        dataRef.Child("users").Child(user.UserId).Child("adrressdata").SetRawJsonValueAsync(adrressjson)
            .ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        LoadManager.Loading(false);
                        if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                        return;
                    }

                    if (result.IsCompleted) SetPatientData();
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

        var patient = new PatientData()
        {
            name1 = name1Input.text,
            relationship1 = relationship1Input.text,
            phone1 = phone1Input.text,
            name2 = name2Input.text,
            relationship2 = relationship2Input.text,
            phone2 = phone2Input.text,
        };

        var patientjson = JsonUtility.ToJson(patient);

        dataRef.Child("users").Child(user.UserId).Child("patientdata").SetRawJsonValueAsync(patientjson)
            .ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        LoadManager.Loading(false);
                        if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                        return;
                    }

                    if (result.IsCompleted) SetMedicalData();
                });
    }

    private void SetMedicalData()
    {
        var medical = new MedicalData()
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
        var medicaljson = JsonUtility.ToJson(medical);

        dataRef.Child("users").Child(user.UserId).Child("medicaldata").SetRawJsonValueAsync(medicaljson)
            .ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        LoadManager.Loading(false);
                        if (result.Exception != null) Debug.LogError(result.Exception.InnerExceptions);
                        return;
                    }

                    if (result.IsCompleted) SetVerify();
                });
    }

    private void SetVerify()
    {
        dataRef.Child("users").Child(user.UserId).Child("userverify")
            .UpdateChildrenAsync(new Dictionary<string, object>() { { "isPatientVerify", true } })
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
                        SceneManager.LoadScene("CharacterSelectScene");
                    }
                });
    }
}