using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class ProfileManager
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

    [SerializeField] private Button dataButton, backButton;
}
