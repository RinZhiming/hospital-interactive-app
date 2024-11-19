using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LocalizationAddress : MonoBehaviour
{
    [Header("Json")]
    [SerializeField] private TextAsset jsonAddress;

    [Header("Dropdown")]
    [SerializeField] private TMP_Dropdown provinceDropdown;
    [SerializeField] private TMP_Dropdown amphureDropdown;
    [SerializeField] private TMP_Dropdown tambonDropdown;
    public bool IsInitDone { get; private set; }

    private Address[] address;

    private void Awake()
    {
        IsInitDone = false;
        
        address = JsonHelper.GetJsonArray<Address>(jsonAddress.text);
        StartCoroutine(AddressDelay());
    }

    private IEnumerator AddressDelay()
    {
        yield return new WaitUntil(() => address.Length >= 77);
        yield return new WaitForSeconds(0.1f);
        InitAddress();
    }

    private void InitAddress()
    {
        ProvinceDropdown();
        AmphureDropdown(0);
        TambonDropdown(0, 0);
        IsInitDone = true;
    }

    private void Start()
    {
        provinceDropdown.onValueChanged.AddListener(i =>
        {
            AmphureDropdown(i);
            TambonDropdown(i, amphureDropdown.value);
        });
        amphureDropdown.onValueChanged.AddListener(i =>
        {
            TambonDropdown(provinceDropdown.value, i);
        });
    }

    private void ProvinceDropdown()
    {
        provinceDropdown.ClearOptions();
        
        foreach (var a in address)
        {
            provinceDropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
                { new() { text = LocalizationSystem.LocalizationLanguage == Language.English ? a.name_en : a.name_th } });
        }
        
    }

    private void AmphureDropdown(int i)
    {
        amphureDropdown.ClearOptions();
        
        foreach (var am in address[i].amphure)
        {
            amphureDropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
                { new() { text = LocalizationSystem.LocalizationLanguage == Language.English ? am.name_en : am.name_th } });
        }
    }

    private void TambonDropdown(int i, int j)
    {
        tambonDropdown.ClearOptions();
        
        foreach (var tm in address[i].amphure[j].tambon)
        {
            tambonDropdown.AddOptions(new List<TMP_Dropdown.OptionData>()
                { new() { text = LocalizationSystem.LocalizationLanguage == Language.English ? tm.name_en : tm.name_th } });
        }
    }
}

[Serializable]
public class Address
{
    public int id;
    public string name_th;
    public string name_en;
    public int geography_id;
    public string created_at;
    public string updated_at;
    public string deleted_at;
    public Amphure[] amphure;
}

[Serializable]
public class Amphure
{
    public int id;
    public string name_th;
    public string name_en;
    public int province_id;
    public string created_at;
    public string updated_at;
    public string deleted_at;
    public Tambon[] tambon;
}

[Serializable]
public class Tambon
{
    public int id;
    public int zip_code;
    public string name_th;
    public string name_en;
    public int amphure_id;
    public string created_at;
    public string updated_at;
    public string deleted_at;
}