using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class NurseSeeDoctorManager : MonoBehaviour
{
    [SerializeField] private List<AppointmentClone> appointments = new();
    private DoctorData currentDoctor;
    private AppointmentUserClone currentAppointmentUser;
    public static AppointmentClone CurrentAppointment { get; set; }
    private DateTime dateTime;
    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    private DateTimeFormatInfo info;

    #region UnityEvent
    
    private void Awake()
    {
        SeeDoctorEvent.OnEnterSeeDoctor += OnEnterSeeDoctor;
        
        yesDoctorButton.onClick.AddListener(() =>
        {
            seeDoctorGroup.SetActive(false);
            confirmSeeDoctorGroup.SetActive(true);
        });

        gotoDoctorButton.onClick.AddListener(GoToButton);
        
        changeTimeButton.onClick.AddListener(() =>
        {
            GetAllAppointment();
            seeDoctorGroup.SetActive(false);
            changeTimeGroup.SetActive(true);
        });
        
        changeTimeBackButton.onClick.AddListener((() =>
        {
            seeDoctorGroup.SetActive(true);
            changeTimeGroup.SetActive(false);
        }));
        
        confirmChangeTimeBackButton.onClick.AddListener(ConfirmChangeTimeBackButton);
        confirmChangeTimeButton.onClick.AddListener(ConfirmChangeTimeButton);
        enterRoomButton.onClick.AddListener(EnterRoomButton);
        waitEnterRoomButton.onClick.AddListener(WaitEnterRoomButton);
        mainSeeDoctorGroup.SetActive(false);
        seeDoctorGroup.SetActive(false);
        confirmSeeDoctorGroup.SetActive(false);
        changeTimeGroup.SetActive(false);
        confirmChangeTimeGroup.SetActive(false);
        enterRoomGroup.SetActive(false);
        
        
        foreach (var button in closeButtons)
        {
            button.onClick.AddListener(CloseButton);
        }
        
        TimeAppointmentObject.OnClick += OnClick;
        SeeDoctorEvent.OnPlayerTrigger += ChangeSceneTrigger;
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;

        info = new();
        dateTime = new DateTime(2024, 11, 1, 15,5,0); //Change HERE!!
        //new DateTime(2024, 10, 23, 15,5,0)
    }

    private void OnDestroy()
    {
        TimeAppointmentObject.OnClick -= OnClick;
        SeeDoctorEvent.OnEnterSeeDoctor -= OnEnterSeeDoctor;
        SeeDoctorEvent.OnPlayerTrigger -= ChangeSceneTrigger;
    }
    
    #endregion
    
    #region GetAppointment
    
    private void OnEnterSeeDoctor()
    {
        seeDoctorText.text = string.Empty;
        confirmSeeDoctorText.text = string.Empty;
        
        seeDoctorGroup.SetActive(true);
        
        GetDoctorAppointment();
        
        if (currentDoctor == null)
        {
            seeDoctorText.text = $"คุณยังไม่มีนัดในขณะนี้";
            changeTimeButton.interactable = false;
            yesDoctorButton.interactable = false;
        }
    }
    
    private void GetDoctorAppointment()
    {
        
        dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("appointments").GetValueAsync().ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                Debug.LogError(result.Exception.Message);
                return;
            }

            if (result.IsCompleted)
            {
                Debug.Log("Test");
                ClearChild();
                appointments.Clear();
                CurrentAppointment = null;
                currentAppointmentUser = null;
                currentDoctor = null;
                
                foreach (var appointmentChildren in result.Result.Children)
                {
                    var newappointment = JsonUtility.FromJson<AppointmentUser>(appointmentChildren.GetRawJsonValue());
                    var rawdate = newappointment.date.Split('-');
                    var date = new DateTime(int.Parse(rawdate[2]), int.Parse(rawdate[1]), int.Parse(rawdate[0]));
                    
                    if (date.Date == dateTime.Date)
                    {
                        var seperateTime = newappointment.time.Split('-');
                        var start = seperateTime[0].Split('.')[0];
                        var end = seperateTime[1].Split('.')[0];
                        
                        var startTime = new TimeSpan(int.Parse(start), 0,0);
                        var endTime = new TimeSpan(int.Parse(end), 0,0);

                        if (dateTime.Hour == startTime.Hours && dateTime.Hour < endTime.Hours)
                        {
                            if (!newappointment.isExamination)
                            {
                                Debug.Log("Found Appointment");
                                currentAppointmentUser = new AppointmentUserClone()
                                {
                                    date = date,
                                    doctorId = newappointment.doctorId,
                                    isExamination = newappointment.isExamination,
                                    startTime = startTime,
                                    endTime = endTime,
                                    key = appointmentChildren.Key,
                                };
                                break;
                            }
                        }
                    }
                    
                }
                
                Debug.Log("GetDoctorAppointment " + currentAppointmentUser.doctorId);
                GetCurrentAppointment();
            }
        });
    }
    
    private void GetCurrentAppointment()
    { 
        dbRef.Child("appointments").Child(currentAppointmentUser.doctorId).GetValueAsync().ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                Debug.LogError(result.Exception.Message);
                return;
            }

            if (result.IsCompleted)
            {
                foreach (var appointmentChildren in result.Result.Children)
                {
                    var rawdate = appointmentChildren.Key.Split('-');
                    var date = new DateTime(int.Parse(rawdate[2]), int.Parse(rawdate[1]), int.Parse(rawdate[0]));
                    if (date.Date == currentAppointmentUser.date.Date)
                    {
                        foreach (var appointment in appointmentChildren.Children)
                        {
                            var appointmentData = appointment.GetRawJsonValue();
                            var newappointment = JsonUtility.FromJson<Appointment>(appointmentData);
                            
                            var seperateTime = newappointment.time.Split('-');
                            var start = seperateTime[0].Split('.')[0];
                            var end = seperateTime[1].Split('.')[0];
                        
                            var startTime = new TimeSpan(int.Parse(start), 0,0);
                            var endTime = new TimeSpan(int.Parse(end), 0,0);

                            if (currentAppointmentUser.startTime == startTime && currentAppointmentUser.endTime == endTime)
                            {
                                if (CurrentAppointment == null)
                                {
                                    CurrentAppointment = new AppointmentClone
                                    {
                                        channelName = newappointment.channelName,
                                        date = date,
                                        doctorId = newappointment.doctorId,
                                        hasAppointment = newappointment.hasAppointment,
                                        hasEnd = newappointment.hasEnd,
                                        patientId = newappointment.patientId,
                                        sort = newappointment.sort,
                                        startTime = startTime,
                                        endTime = endTime,
                                        key = appointment.Key,
                                    };
                                    break;
                                }
                            }
                        }
                    }
                    
                    
                }
                
                Debug.Log("GetAppointment " + CurrentAppointment.doctorId);
                GetCurrentDoctorData();
            }
        });

    }
    
    private void GetCurrentDoctorData()
    {
        dbRef.Child("doctors").Child(CurrentAppointment.doctorId).GetValueAsync().ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                Debug.LogError(result.Exception.Message);
                return;
            }

            if (result.IsCompleted)
            {                
                var rawdata = result.Result.GetRawJsonValue();
                var data = JsonUtility.FromJson<DoctorData>(rawdata);

                if (currentDoctor == null)
                {
                    currentDoctor = data;
                    Debug.Log("GetDoctorData " + currentDoctor.doctorId);
                    SetCurrentAppointmentUi();
                }
                
            }
        });
    }

    private void SetCurrentAppointmentUi()
    {
        if (CurrentAppointment != null)
        {
            seeDoctorText.text = $"นัดของคุณวันนี้คือ {currentDoctor.firstName} {currentDoctor.lastName} เวลา {CurrentAppointment.startTime.Hours}:00 - {CurrentAppointment.endTime.Hours}:00 ใช่หรือไม่";
            confirmSeeDoctorText.text = $"คุณสามารถรอพบ {currentDoctor.firstName} {currentDoctor.lastName}  ได้ที่หน้าห้องตรวจค่ะ";
            changeTimeButton.interactable = true;
            yesDoctorButton.interactable = true;
        }
    }

    private void GetAllAppointment()
    {
        dbRef.Child("appointments").Child(CurrentAppointment.doctorId).GetValueAsync().ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                Debug.LogError(result.Exception.Message);
                return;
            }

            if (result.IsCompleted)
            {
                var handleAppointment = new List<AppointmentClone>();
                foreach (var appointmentChildren in result.Result.Children)
                {
                    var rawdate = appointmentChildren.Key.Split('-');
                    var date = new DateTime(int.Parse(rawdate[2]), int.Parse(rawdate[1]), int.Parse(rawdate[0]));
                    
                    if (date.Date >= dateTime.Date)
                    {
                        foreach (var appointmentDataSnapshot in appointmentChildren.Children)
                        {
                            var appointmentData = appointmentDataSnapshot.GetRawJsonValue();
                            var newappointment = JsonUtility.FromJson<Appointment>(appointmentData);
                            
                            var seperateTime = newappointment.time.Split('-');
                            var start = seperateTime[0].Split('.')[0];
                            var end = seperateTime[1].Split('.')[0];
                            
                            var startTime = new TimeSpan(int.Parse(start), 0,0);
                            var endTime = new TimeSpan(int.Parse(end), 0,0);
                            
                            if (!newappointment.hasAppointment && !newappointment.hasEnd)
                            {
                                var newappointmentclone = new AppointmentClone
                                {
                                    channelName = newappointment.channelName,
                                    date = date,
                                    doctorId = newappointment.doctorId,
                                    hasAppointment = newappointment.hasAppointment,
                                    hasEnd = newappointment.hasEnd,
                                    patientId = newappointment.patientId,
                                    sort = newappointment.sort,
                                    startTime = startTime,
                                    endTime = endTime,
                                    key = appointmentDataSnapshot.Key,
                                };
                                
                                if (endTime.Hours > currentAppointmentUser.endTime.Hours && date.Date == dateTime.Date)
                                {
                                    if(!handleAppointment.Contains(newappointmentclone))
                                        handleAppointment.Add(newappointmentclone);
                                }

                                if (date.Date > dateTime.Date)
                                {
                                    if(!handleAppointment.Contains(newappointmentclone))
                                        handleAppointment.Add(newappointmentclone);
                                }
                            }
                        }
                    }
                    
                }
                handleAppointment.Sort();
                
                appointments.AddRange(handleAppointment);
                
                Debug.Log("All Appointment : " + appointments.Count);
                SetUiAppointment();
            }
        });

    }

    private void SetUiAppointment()
    {
        if (appointments.Count > 0)
        {
            foreach (var appointment in appointments)
            {
                var ap = Instantiate(appointmentPrefab, timeAppointment);
                var date = appointment.date;
                var appointmentObj = ap.GetComponent<TimeAppointmentObject>();
                appointmentObj.DateText.text = $"{date.DayOfWeek} {date.Day} {info.GetMonthName(date.Month)} {date.Year}";
                appointmentObj.TimeText.text = $"{appointment.startTime.Hours}:00 - {appointment.endTime.Hours}:00";
                appointmentObj.Appointment = appointment;
            }   
        }
    }
    
    #endregion
    
    #region SetAppointment

    private void OnClick(TimeAppointmentObject appointment)
    {
        GetDoctorName(appointment.Appointment);
        
        changeTimeGroup.SetActive(false);
        confirmChangeTimeGroup.SetActive(true);
    }

    private void GetDoctorName(AppointmentClone appointment)
    {
        dbRef.Child("doctors").Child(appointment.doctorId).GetValueAsync().ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                Debug.LogError(result.Exception.Message);
                return;
            }

            if (result.IsCompleted)
            {                
                var rawdata = result.Result.GetRawJsonValue();
                var data = JsonUtility.FromJson<DoctorData>(rawdata);

                doctorText.text = $"แพทย์: {data.firstName} {data.lastName}";
                dateText.text = $"วันที่: {appointment.date.DayOfWeek} {appointment.date.Day} {info.GetMonthName(appointment.date.Month)} {appointment.date.Year}";
                timeText.text = $"เวลา: {appointment.startTime.Hours}:00 - {appointment.endTime.Hours}:00";
            }
        });
    }
    
    private void ConfirmChangeTimeBackButton()
    {
        changeTimeGroup.SetActive(true);
        confirmChangeTimeGroup.SetActive(false);
        CurrentAppointment = null;
    }

    private void ConfirmChangeTimeButton()
    {
        DeleteCurrentAppointmentUser();
    }

    private void DeleteCurrentAppointmentUser()
    {
        dbRef.Child("users").Child(auth.CurrentUser.UserId).Child("appointments").Child(currentAppointmentUser.key)
            .RemoveValueAsync().ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        Debug.LogError(result.Exception.Message);
                        return;
                    }
                    
                    if (result.IsCompleted)
                    {
                        DeleteCurrentAppointmentTime();
                    }
                });
    }

    private void DeleteCurrentAppointmentTime()
    {
        var date = $"{CurrentAppointment.date.Day}-{CurrentAppointment.date.Month}-{CurrentAppointment.date.Year}";
        dbRef.Child("appointments").Child(CurrentAppointment.doctorId).Child(date).Child(CurrentAppointment.key)
            .RemoveValueAsync().ContinueWithOnMainThread(
                result =>
                {
                    if (result.IsFaulted || result.IsCanceled)
                    {
                        Debug.LogError(result.Exception.Message);
                        return;
                    }
                    
                    if (result.IsCompleted)
                    {
                        ChangeTimeSuccess();
                    }
                });
    }

    private void ChangeTimeSuccess()
    {
        confirmChangeTimeGroup.SetActive(false);
        OnEnterSeeDoctor();
    }

    #endregion

    #region Scene

    private void ChangeSceneTrigger()
    {
        enterRoomGroup.SetActive(true);
    }
    
    private void WaitEnterRoomButton()
    {
        enterRoomGroup.SetActive(false);
    }
    
    private void EnterRoomButton()
    {
        enterRoomButton.interactable = false;
        waitEnterRoomButton.interactable = false;
        
        GameManager.Launcher.OnLeftRoom();
        SceneManager.LoadScene(SceneName.AppointmentScene.ToString());
    }
    
    #endregion
    
    #region Other

    private void CloseButton()
    {
        SeeDoctorEvent.OnCloseSeeDoctor?.Invoke();
        seeDoctorGroup.SetActive(false);
        confirmSeeDoctorGroup.SetActive(false);
        changeTimeGroup.SetActive(false);
        confirmChangeTimeGroup.SetActive(false);
    }
    

    private void GoToButton()
    {
        SeeDoctorEvent.OnGoSeeDoctor?.Invoke();
        mainSeeDoctorGroup.SetActive(false);
        confirmSeeDoctorGroup.SetActive(false);
    }

    private void ClearChild()
    {
        if (timeAppointment.childCount <= 0) return;

        for (int i = 0; i < timeAppointment.childCount; i++)
        {
            Destroy(timeAppointment.GetChild(i).gameObject);
        }
    }
    

    #endregion
}
