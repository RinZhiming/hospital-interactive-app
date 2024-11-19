using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class NurseAppointmentManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    private DoctorData currentDoctor;
    private AppointmentClone currentAppointment;
    private DateTimeFormatInfo info;
    private Dictionary<AppointmentClone, string> appointmentsMapper = new();
    private bool canGetAppointment, isSubscribe;

    #region UnityEvent
    
    private void Awake()
    {
        appointmentButton.onClick.AddListener(() =>
        {
            GetDoctor();
            Appointment(true);
            canGetAppointment = true;
        });
        
        doctorNextButton.onClick.AddListener(() =>
        {
            if (currentDoctor != null)
            {
                doctorCanvasGroup.SetActive(false);
                timeCanvasGroup.SetActive(true);
                OnGetAppointment();
                currentAppointment = null;
            }
        });
        
        timeNextButton.onClick.AddListener(() =>
        {
            if (currentAppointment != null)
            {
                SummarizeAppointment();
                timeCanvasGroup.SetActive(false);
                confirmCanvasGroup.SetActive(true);
            }
        });
        
        confirmButton.onClick.AddListener(ConfirmButton);
        
        appointmentHomeButton.onClick.AddListener(() => Appointment(false));
        
        doctorBackButton.onClick.AddListener(() =>
        {
            ClearAllChild(doctorContainer);
            currentAppointment = null;
            mainDialogueGroup.SetActive(true);
            appointmentDialogueGroup.SetActive(false);
            canGetAppointment = false;
            if (currentDoctor != null)
            {
                currentDoctor = null;
                isSubscribe = false;
            }
        });
        
        timeBackButton.onClick.AddListener(() =>
        {
            doctorCanvasGroup.SetActive(true);
            timeCanvasGroup.SetActive(false);
        });
        
        confirmBackButton.onClick.AddListener(() =>
        {
            timeCanvasGroup.SetActive(true);
            confirmCanvasGroup.SetActive(false);
        });
        
        appointmentDialogueGroup.SetActive(false);
        doctorCanvasGroup.SetActive(false);
        timeCanvasGroup.SetActive(false);
        confirmCanvasGroup.SetActive(false);
        successConfirmCanvasGroup.SetActive(false);
        
        DoctorAppointmentObject.OnToggle += OnToggle;
        DateTimeAppointmentObject.OnToggle += OnDateTimeToggle;

        info = new DateTimeFormatInfo();
    }
    
    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        doctorNextButton.interactable = false;
        timeNextButton.interactable = false;
    }

    private void OnDestroy()
    {
        DoctorAppointmentObject.OnToggle -= OnToggle;
        DateTimeAppointmentObject.OnToggle -= OnDateTimeToggle;
    }
    
    #endregion
    private void ConfirmButton()
    {
        confirmButton.interactable = false;
        CheckAppointmentBeforeAdd();
    }

    private void CheckAppointmentBeforeAdd()
    {
        var key = appointmentsMapper[currentAppointment];
        var date = $"{currentAppointment.date.Day}-{currentAppointment.date.Month}-{currentAppointment.date.Year}";
        
        dbRef.Child("appointments").Child(currentDoctor.doctorId).Child(date).Child(key).GetValueAsync().ContinueWithOnMainThread(
            result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    Debug.LogError(result.Exception?.Message);
                    return;
                }

                if (result.IsCompleted)
                {
                    var rawdata = result.Result.GetRawJsonValue();
                    var data = JsonUtility.FromJson<Appointment>(rawdata);
                    
                    if (!data.hasAppointment && !data.hasEnd)
                    {
                        Debug.Log("Check Success");
                        UpdateAppointment(key);
                    }
                    else
                    {
                        Debug.LogError("this Appointment no longer Available");
                        currentAppointment = null;
                        confirmButton.interactable = true;
                        confirmCanvasGroup.SetActive(false);
                        timeCanvasGroup.SetActive(true);
                        OnGetAppointment();
                    }
                }
            });
    }

    private void UpdateAppointment(string key)
    {
        Debug.Log($"UpdateAppointment {currentAppointment != null}");
        Debug.Log($"UpdateAppointment {currentDoctor != null}");
        var date = $"{currentAppointment.date.Day}-{currentAppointment.date.Month}-{currentAppointment.date.Year}";
        var time = $"{currentAppointment.startTime.Hours}.00-{currentAppointment.endTime.Hours}.00";
        var newAppointment = new Appointment
        {
            sort = currentAppointment.sort,
            date = date,
            time = time,
            channelName = currentAppointment.doctorId + auth.CurrentUser.UserId,
            doctorId = currentAppointment.doctorId,
            patientId = auth.CurrentUser.UserId,
            hasAppointment = true,
            hasEnd = false,
        };
        
        var appointment = JsonUtility.ToJson(newAppointment);
        
        Debug.Log("On Update Appointment");
        
        dbRef.Child("appointments").Child(currentDoctor.doctorId).Child(date).Child(key).SetRawJsonValueAsync(appointment).ContinueWithOnMainThread(
            result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    Debug.LogError(result.Exception?.Message);
                    return;
                }

                if (result.IsCompleted)
                {
                    Debug.Log("Update Success");
                    UpdateAppointmentUser(date, time, newAppointment.doctorId);
                }
            });
    }

    private void UpdateAppointmentUser(string date, string time, string doctorid)
    {
        var newAppointment = new AppointmentUser
        {
            doctorId = doctorid,
            isExamination = false,
            date = date,
            time = time
        };
        
        var appointment = JsonUtility.ToJson(newAppointment);
        
        Debug.Log("On Update User");
        
        dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("appointments").Push().SetRawJsonValueAsync(appointment).ContinueWithOnMainThread(
            result =>
            {
                if (result.IsFaulted || result.IsCanceled)
                {
                    Debug.LogError(result.Exception?.Message);
                    return;
                }

                if (result.IsCompleted)
                {
                    Debug.Log("Update User Success");
                    Success();
                }
            });
    }

    private void Success()
    {
        confirmCanvasGroup.SetActive(false);
        appointmentDialogueGroup.SetActive(false);
        successConfirmCanvasGroup.SetActive(true);
        confirmButton.interactable = true;
        StartCoroutine(DelaySuccess());
    }

    private IEnumerator DelaySuccess()
    {
        yield return new WaitForSeconds(1.5f);
        currentAppointment = null;
        if (currentDoctor != null)
        {
            currentDoctor = null;
            isSubscribe = false;
        }
        canGetAppointment = false;
        successConfirmCanvasGroup.SetActive(false);
        mainDialogueGroup.SetActive(true);
    }

    private void Update()
    {
        timeNextButton.interactable = currentAppointment != null;
        doctorNextButton.interactable = currentDoctor != null;
        confirmButton.interactable = currentDoctor != null && currentAppointment != null;
    }

    private void OnDateTimeToggle(DateTimeAppointmentObject datetime, bool toggle)
    {
        currentAppointment = toggle ? datetime.Appointment : null;
    }

    private void OnToggle(DoctorAppointmentObject doctor, bool toggle)
    {
        currentDoctor = toggle ? doctor.Doctor : null;
    }

    private void SummarizeAppointment()
    {
        doctorNameText.text = $"แพทย์ : {currentDoctor.firstName} {currentDoctor.lastName}";
        dateText.text = $"วันที่ : {currentAppointment.date.DayOfWeek} {currentAppointment.date.Day} {info.GetMonthName(currentAppointment.date.Month)} {currentAppointment.date.Year}";
        timeText.text = $"เวลา : {currentAppointment.startTime.Hours}:00 - {currentAppointment.endTime.Hours}:00";
    }
    
    
    private void GetDoctor()
    {
        dbRef.Child("doctors").GetValueAsync().ContinueWithOnMainThread(result =>
        {
            if (result.IsCanceled || result.IsFaulted)
            {
                Debug.Log(result.Exception.Message);
                return;
            }
            
    
            if (result.IsCompleted)
            {
                currentDoctor = null;
                ClearAllChild(doctorContainer);
                var toggleGroup = doctorContainer.GetComponent<ToggleGroup>();
                
                var data = result.Result;
                foreach (var doctor in data.Children)
                {
                    var newDoctor = JsonUtility.FromJson<DoctorData>(doctor.GetRawJsonValue());
                    
                    GameObject d = Instantiate(doctorPrefab, doctorContainer);
                    
                    var doctorObj = d.GetComponent<DoctorAppointmentObject>();
                    
                    doctorObj.NameText.text = $"{newDoctor.firstName} {newDoctor.lastName}";
                    doctorObj.Icon.sprite = newDoctor.iconProfile > -1 ? icons[newDoctor.iconProfile - 6] : null;
                    doctorObj.Doctor = newDoctor;
                    
                    if (toggleGroup) doctorObj.Toggle.group = toggleGroup;
                }
            }
        });
    }
    
    private void OnGetAppointment()
    {
        dbRef.Child("appointments").Child(currentDoctor.doctorId).GetValueAsync().ContinueWithOnMainThread(result =>
        {
            if (result.IsCanceled || result.IsFaulted)
            {
                Debug.LogError("Get appointment error");
                return;
            }

            if (result.IsCompleted)
            {
                currentAppointment = null;
                ClearAllChild(dateContainer);
                appointmentsMapper.Clear();
                var toggleGroup = dateContainer.GetComponent<ToggleGroup>();
                var data = result.Result;
                
                foreach (var appointment in data.Children)
                {
                    var rawdate = appointment.Key.Split('-');
                    var date = new DateTime(int.Parse(rawdate[2]), int.Parse(rawdate[1]), int.Parse(rawdate[0]));

                    var newAppointmentData = JObject.Parse(appointment.GetRawJsonValue());
                    var handleAppointment = new List<AppointmentClone>();
                    
                    foreach (var appointmentData in newAppointmentData)
                    {
                        var newAppointment = JsonUtility.FromJson<Appointment>(appointmentData.Value.ToString());
                        var seperateTime = newAppointment.time.Split('-');
                        var start = seperateTime[0].Split('.')[0];
                        var end = seperateTime[1].Split('.')[0];
                        
                        var startTime = new TimeSpan(int.Parse(start), 0,0);
                        var endTime = new TimeSpan(int.Parse(end), 0,0);
                        var currentAppointmentData = new AppointmentClone
                        {
                            channelName = newAppointment.channelName,
                            date = date,
                            doctorId = newAppointment.doctorId,
                            startTime = startTime,
                            endTime = endTime,
                            hasAppointment = newAppointment.hasAppointment,
                            hasEnd = newAppointment.hasEnd,
                            key = appointmentData.Key,
                            patientId = newAppointment.patientId,
                            sort = newAppointment.sort
                        };
                        if (!handleAppointment.Contains(currentAppointmentData)) handleAppointment.Add(currentAppointmentData);
                        appointmentsMapper.TryAdd(currentAppointmentData, appointmentData.Key);
                    }
                    
                    handleAppointment.Sort();
                    
                    foreach (var appointmentSort in handleAppointment)
                    {
                        if (!appointmentSort.hasAppointment && date.Date >= DateTime.Now.Date)
                        {
                            var ap = Instantiate(datePrefab, dateContainer);
                            var appointmentObj = ap.GetComponent<DateTimeAppointmentObject>();
                            appointmentObj.DateText.text = $"{date.DayOfWeek} {date.Day} {info.GetMonthName(date.Month)} {date.Year}";
                            appointmentObj.TimeText.text = $"{appointmentSort.startTime.Hours}:00 - {appointmentSort.endTime.Hours}:00";
                            appointmentObj.Appointment = appointmentSort;
                            
                            if (toggleGroup) appointmentObj.Toggle.group = toggleGroup;
                        }
                    }
                }
            }
        });
    }

    private void Appointment(bool show)
    {
        mainDialogueGroup.SetActive(!show);
        appointmentDialogueGroup.SetActive(show);
        doctorCanvasGroup.SetActive(show);
    }
    
    private void ClearAllChild(Transform parent)
    {
        if (!parent) return;
        
        if (parent.childCount <= 0) return;
        
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
    }
}
