using System;

[Serializable]
public class GeneralData
{
    public string cardIdentity;
    public int sex;
    public string nationality;
    public int religion;
    public int state;
    public string job;
    public string housePhone;
    public string mobilePhone;
    public string email;
}

[Serializable]
public class AddressData
{
    public string houseNumber;
    public string villageNumber;
    public string village;
    public string alley;
    public string road;
    public int provinceNumber;
    public int district;
    public int subDistrict;
    public string postalCode;
}

[Serializable]
public class PatientData
{
    public string name1;
    public string relationship1;
    public string phone1;
    public string name2;
    public string relationship2;
    public string phone2;
}

[Serializable]
public class MedicalData
{
    public bool unknowAllergy;
    public bool noAllergy;
    public bool allergyMedicine;
    public bool allergyFood;
    public bool patient;
    public bool relative;
    public bool other;
    public string allergyMedicineInfo;
    public string allergyFoodInfo;
    public string otherNameInfo;
    public string otherPhoneInfo;
}
