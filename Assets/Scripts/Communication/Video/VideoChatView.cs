using System;
using Agora.Rtc;
using UnityEngine;

public partial class VideoChatView : MonoBehaviour
{
    private AppointmentClone appointment;
    
    private void Awake()
    {
        joinButton.onClick.AddListener(JoinButton);
        leaveButton.onClick.AddListener(LeaveButton);
        muteMicrophoneButton.onClick.AddListener(MuteMicrophoneButton);
        muteSpeakerButton.onClick.AddListener(MuteSpeakerButton);
        videoChatCanvasGroup.SetActive(false);
        
        VideoEvent.OnJoinChannelSuccessful += OnJoinChannelSuccessful;
        VideoEvent.OnLeaveChannelSuccessful += OnLeaveChannelSuccessful;
        VideoEvent.OnInitSuccessful += OnInitSuccessful;
        VideoEvent.RemoteUserJoin += RemoteUserJoin;
        VideoEvent.RemoteUserLeave += RemoteUserLeave;
        VideoEvent.OnError += OnError;
        VideoEvent.LocalUserJoin += LocalUserJoin;
    }

    private void Start()
    {
        joinButton.interactable = false;
        appointment = NurseSeeDoctorManager.CurrentAppointment;
        SetUpUi();
    }
    
    private void OnDestroy()
    {
        VideoEvent.OnJoinChannelSuccessful -= OnJoinChannelSuccessful;
        VideoEvent.OnLeaveChannelSuccessful -= OnLeaveChannelSuccessful;
        VideoEvent.RemoteUserJoin -= RemoteUserJoin;
        VideoEvent.OnInitSuccessful -= OnInitSuccessful;
        VideoEvent.RemoteUserLeave -= RemoteUserLeave;
        VideoEvent.OnError -= OnError;
        VideoEvent.LocalUserJoin -= LocalUserJoin;
    }
    
    private void RemoteUserJoin(uint uid, string channelName, VIDEO_SOURCE_TYPE vidtype)
    {
        Debug.Log(uid);
        VideoChatController.CurrentDoctorId = uid;
        doctorVideo.SetEnable(true);
        doctorVideo.SetForUser(uid, channelName, vidtype);
    }
    
    private void RemoteUserLeave()
    {
        doctorVideo.SetEnable(false);
    }

    private void SetUpUi()
    {
        doctorCamImage.transform.localEulerAngles = new Vector3(0,0,180);
        patientCamImage.transform.localEulerAngles = new Vector3(0,0,180);
        
        patientVideo = patientCamImage.gameObject.AddComponent<VideoSurface>();
        doctorVideo = doctorCamImage.gameObject.AddComponent<VideoSurface>();
        
        patientVideo.SetEnable(false);
        doctorVideo.SetEnable(false);
    }
    
    private void LocalUserJoin()
    {
        patientVideo.SetForUser(0, string.Empty);
    }
    
    private void OnInitSuccessful()
    {
        joinButton.interactable = true;
    }
    
    private void MuteSpeakerButton()
    {
        VideoEvent.MuteSpeaker?.Invoke();
    }

    private void MuteMicrophoneButton()
    {
        VideoEvent.MuteMicrophone?.Invoke();
    }

    private void JoinButton()
    {
        joinButton.interactable = false;
        VideoEvent.OnJoinChannel?.Invoke(appointment.channelName);
    }

    private void LeaveButton()
    {
        leaveButton.interactable = false;
        VideoEvent.OnLeaveChannel?.Invoke();
        patientVideo.SetEnable(false);
        doctorVideo.SetEnable(false);
    }
    
    private void OnJoinChannelSuccessful()
    {
        joinButton.interactable = true;
        videoChatCanvasGroup.SetActive(true);
        patientVideo.SetEnable(true);
    }

    private void OnLeaveChannelSuccessful()
    {
        leaveButton.interactable = true;
        videoChatCanvasGroup.SetActive(false);
    }
    
    private void OnError()
    {
        joinButton.interactable = true;
    }
}