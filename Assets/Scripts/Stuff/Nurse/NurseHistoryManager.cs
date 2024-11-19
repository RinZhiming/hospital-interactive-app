using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public partial class NurseHistoryManager : MonoBehaviour
{
    private Dictionary<string, List<MedicalSummaryClone>> medicals = new();
    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    
    private void Awake()
    {
        historyButton.onClick.AddListener(HistoryButton);
        medicalBackButton.onClick.AddListener(MedicalBackButton);
        closeButton.onClick.AddListener(CloseButton);
        doctorBackButton.onClick.AddListener(CloseButton);
        dateBackButton.onClick.AddListener(DateBackButton);
        
        HistoryDoctorObject.OnClick += OnDoctorClick;
        HistoryDateObject.OnClick += OnDateClick;
    }

    private void OnDestroy()
    {
        HistoryDoctorObject.OnClick -= OnDoctorClick;
        HistoryDateObject.OnClick -= OnDateClick;
    }

    private void Start()
    {
        ClearChild(doctorContainer);
        ClearChild(dateContainer);
        ClearChild(medicalContainer);
        
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        
        doctorCanvasGroup.SetActive(false);
        dateCanvasGroup.SetActive(false);
        medicalCanvasGroup.SetActive(false);
        historyCanvasGroup.SetActive(false);
    }
    
    private void OnDoctorClick(HistoryDoctorObject obj)
    {
        doctorCanvasGroup.SetActive(false);
        dateCanvasGroup.SetActive(true);
        GetMedical(obj.Doctor.doctorId);
    }
    
    private void OnDateClick(HistoryDateObject obj)
    {
        doctorCanvasGroup.SetActive(false);
        dateCanvasGroup.SetActive(false);
        medicalCanvasGroup.SetActive(true);
        patientNameText.text = $"ผู้ป่วย {obj.MedicalSummary.patientName}";
        dateText.text = $"วันที่ {obj.MedicalSummary.dateTime.Day}/{obj.MedicalSummary.dateTime.Month}/{obj.MedicalSummary.dateTime.Year}";
        doctorNameText.text = $"ผู้ตรวจ {obj.MedicalSummary.doctorName}";
        pbText.text = $"พ.บ. {obj.MedicalSummary.doctorId}";
        totalText.text = obj.MedicalSummary.totalCost.ToString();
        foreach (var medical in obj.MedicalSummary.medicals)
        {
            var medicalGameobject = Instantiate(medicalPrefab, medicalContainer);
            var medicalObject = medicalGameobject.GetComponent<MedicalObject>();
            medicalObject.Medical = medical;
            medicalObject.NameMedicalInputField.interactable = false;
            medicalObject.CostMedicalInputField.interactable = false;
            medicalObject.NumberMedicalInputField.interactable = false;
            medicalObject.NameMedicalInputField.text = medical.name;
            medicalObject.CostMedicalInputField.text = medical.cost.ToString();
            medicalObject.NumberMedicalInputField.text = medical.number.ToString();
        }
    }
    
    private void DateBackButton()
    {
        ClearChild(dateContainer);
        doctorCanvasGroup.SetActive(true);
        dateCanvasGroup.SetActive(false);
        medicalCanvasGroup.SetActive(false);
    }

    private void CloseButton()
    {
        doctorCanvasGroup.SetActive(false);
        dateCanvasGroup.SetActive(false);
        medicalCanvasGroup.SetActive(false);
        historyCanvasGroup.SetActive(false);
        ClearChild(doctorContainer);
        ClearChild(dateContainer);
        ClearChild(medicalContainer);
        HistoryEvents.OnClose?.Invoke();
    }

    private void MedicalBackButton()
    {
        patientNameText.text = string.Empty;
        dateText.text = string.Empty;
        doctorNameText.text = string.Empty;
        pbText.text = string.Empty;
        ClearChild(medicalContainer);
        medicalCanvasGroup.SetActive(false);
        dateCanvasGroup.SetActive(true);
    }

    private void HistoryButton()
    {
        ClearChild(doctorContainer);
        ClearChild(dateContainer);
        ClearChild(medicalContainer);
        historyCanvasGroup.SetActive(true);
        doctorCanvasGroup.SetActive(true);
        HistoryEvents.OnHistory?.Invoke();
        GetDoctor();
    }

    private void GetDoctor()
    {
        LoadManager.Loading(true);
        medicals.Clear();
        
        var handleData = new Dictionary<string, MedicalSummary>();
        
        dbRef.Child("medicals").GetValueAsync()
            .ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        LoadManager.Loading(false);
                        if (result.Exception != null) 
                            Debug.LogError(result.Exception.Message);
                        return;
                    }

                    if (result.IsCompleted)
                    {
                        foreach (var doctors in result.Result.Children)
                        {
                            foreach (var date in doctors.Children)
                            {
                                foreach (var pushKey in date.Children)
                                {
                                    if (pushKey.Child("patientId").Exists)
                                    {
                                        if (pushKey.Child("patientId").Value.ToString() == auth.CurrentUser.UserId)
                                        {
                                            var data = JsonUtility.FromJson<MedicalSummary>(pushKey.GetRawJsonValue());
                                            handleData.Add(doctors.Key, null);
                                            handleData[doctors.Key] = data;
                                        }
                                    }
                                }
                            }
                        }

                        SetDoctorData(handleData);
                    }
                });
    }

    private void SetDoctorData(Dictionary<string, MedicalSummary> medicaldata)
    {
        var handleDoctorId = new List<string>();
        try
        {
            if (medicaldata.Count > 0)
            {
                foreach (var data in medicaldata)
                {

                    medicals.TryAdd(data.Key, new List<MedicalSummaryClone>());
                    var rawtime = data.Value.time.Split('-');
                    var rawstarttime = rawtime[0].Split('.')[0];
                    var rawendtime = rawtime[1].Split('.')[0];
                    var dateTime = DateTime.Parse(data.Value.dateTime);
                    var starttime = new TimeSpan(int.Parse(rawstarttime), 0, 0);
                    var endtime = new TimeSpan(int.Parse(rawendtime), 0, 0);
                    
                    var newMedical = new MedicalSummaryClone
                    {
                        doctorId = data.Value.doctorId,
                        dateTime = dateTime,
                        doctorName = data.Value.doctorName,
                        medicals = data.Value.medicals,
                        patientId = data.Value.patientId,
                        patientName = data.Value.patientName,
                        startTime = starttime,
                        endTime = endtime,
                        totalCost = data.Value.totalCost
                    };

                    medicals[data.Key].Add(newMedical);
                    if (!handleDoctorId.Contains(data.Key)) 
                        handleDoctorId.Add(data.Key);

                }
                
                StartCoroutine(GetDoctorData(handleDoctorId));
            }
            else
            {
                LoadManager.Loading(false);
            }
        }
        catch (Exception e)
        {
            LoadManager.Loading(false);
        }
    }

    private IEnumerator GetDoctorData(List<string> doctorData)
    {
        var doctorHandle = new List<DoctorData>();
        foreach (var id in doctorData)
        {
            var isComplete = false;
            
            dbRef.Child("doctors").Child(id).GetValueAsync().ContinueWithOnMainThread(result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    LoadManager.Loading(false);
                    if (result.Exception != null) 
                        Debug.LogError(result.Exception.Message);
                    isComplete = true;
                    return;
                }

                if (result.IsCompleted)
                {
                    var data = JsonUtility.FromJson<DoctorData>(result.Result.GetRawJsonValue());
                    if (!doctorHandle.Contains(data)) doctorHandle.Add(data);
                    isComplete = true;
                }
            });
            
            yield return new WaitUntil(() => isComplete);
        }

        DisplayDoctorData(doctorHandle);
    }

    private void DisplayDoctorData(List<DoctorData> doctorsData )
    {
        foreach (var data in doctorsData)
        {
            var newDoctor = Instantiate(doctorPrefab, doctorContainer);
            var doctor = newDoctor.GetComponent<HistoryDoctorObject>();
            doctor.Doctor = data;
            doctor.NameText.text = $"{data.firstName} {data.lastName}";
            doctor.Icon.sprite = icons[data.iconProfile - 6];
        }
        
        LoadManager.Loading(false);
    }

    private void GetMedical(string doctorId)
    {
        LoadManager.Loading(true);

        if (medicals.Count > 0)
        {
            foreach (var medical in medicals)
            {
                if (medical.Key == doctorId)
                {
                    DisplayMedicalSummary(medical.Value);
                    break;
                }
            }
        }
        else
        {
            LoadManager.Loading(false);
        }
    }

    private void DisplayMedicalSummary(List<MedicalSummaryClone> medicals)
    {
        try
        {
            foreach (var data in medicals)
            {
                var newTime = Instantiate(datePrefab, dateContainer);
                var medical = newTime.GetComponent<HistoryDateObject>();
                medical.MedicalSummary = data;
                medical.DateText.text = $"วันที่ {data.dateTime.Day}/{data.dateTime.Month}/{data.dateTime.Year}";
                medical.TimeText.text = $"เวลา {data.startTime.Hours}:00 - {data.endTime.Hours}:00";
            }
            
            LoadManager.Loading(false);

        }
        catch (Exception e)
        {
            LoadManager.Loading(false);
        }
    }
    
    private void ClearChild(Transform container)
    {
        if (container.childCount <= 0) return;

        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }
}
