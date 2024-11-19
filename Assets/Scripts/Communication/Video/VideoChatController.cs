using System;
using System.Collections;
using Agora.Rtc;
using UnityEngine;
using UnityEngine.Android;

public class VideoChatController : MonoBehaviour
{
    [SerializeField] private string appId;
    private VideoChatModel videoChatModel;
    private bool isMuteMic;
    private bool isMuteSpeaker;
    public static uint CurrentDoctorId { get; set; }
    private ArrayList permissionList = new() { Permission.Camera, Permission.Microphone };

    
    private void Awake()
    {
        VideoEvent.OnJoinChannel += OnJoinChannel;
        VideoEvent.OnLeaveChannel += OnLeaveChannel;
        VideoEvent.MuteMicrophone += MuteMicrophone;
        VideoEvent.MuteSpeaker += MuteSpeaker;
        
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        
        videoChatModel = new VideoChatModel();
    }

    private void Start()
    {
        InitData();
    }

    private void OnDestroy()
    {
        VideoEvent.OnJoinChannel -= OnJoinChannel;
        VideoEvent.OnLeaveChannel -= OnLeaveChannel;
    }
    
    private void MuteSpeaker()
    {
        isMuteSpeaker = !isMuteSpeaker;
        videoChatModel.RtcEngine.MuteAllRemoteAudioStreams(isMuteSpeaker);
    }

    private void MuteMicrophone()
    {
        isMuteMic = !isMuteMic;
        videoChatModel.RtcEngine.MuteLocalAudioStream(isMuteMic);
    }

    private void InitData()
    {
        CheckPermissions();
        
        try
        {
            videoChatModel.RtcEngine = RtcEngine.CreateAgoraRtcEngine();
            
            var videoEvent = new VideoChatEvent(this);
            var rtcContext = new RtcEngineContext
            {
                appId = appId,
                channelProfile = CHANNEL_PROFILE_TYPE.CHANNEL_PROFILE_COMMUNICATION,
                audioScenario = AUDIO_SCENARIO_TYPE.AUDIO_SCENARIO_CHATROOM,
                areaCode = AREA_CODE.AREA_CODE_AS
            };
            
            videoChatModel.RtcEngine.Initialize(rtcContext);
            videoChatModel.RtcEngine.InitEventHandler(videoEvent);
            videoChatModel.RtcEngine.EnableVideo();
            videoChatModel.RtcEngine.EnableAudio();
            videoChatModel.RtcEngine.MuteAllRemoteAudioStreams(false);
            videoChatModel.RtcEngine.MuteLocalAudioStream(false);
        }
        finally
        {
            VideoEvent.OnInitSuccessful?.Invoke();
        }
    }

    private void OnJoinChannel(string roomName)
    {
        CheckPermissions();
        
        if (videoChatModel.RtcEngine != null && !string.IsNullOrEmpty(roomName))
        {
            var option = new ChannelMediaOptions();
            option.autoSubscribeAudio.SetValue(true);
            option.autoSubscribeVideo.SetValue(true);
            videoChatModel.RtcEngine.EnableVideo();
            videoChatModel.RtcEngine.EnableAudio();
            videoChatModel.RtcEngine.JoinChannel(string.Empty, roomName, 0, option);
        }
        else
        {
            VideoEvent.OnError?.Invoke();
        }
    }
    
    private void CheckPermissions() 
    {
        foreach (string permission in permissionList)
        {
            if (!Permission.HasUserAuthorizedPermission(permission))
            {
                Permission.RequestUserPermission(permission);
            }
        }
    }

    private void OnLeaveChannel()
    {
        if (videoChatModel.RtcEngine != null)
        {
            videoChatModel.RtcEngine.LeaveChannel();
            videoChatModel.RtcEngine.DisableVideo();
            videoChatModel.RtcEngine.DisableAudio();
        }
    }

    private void OnApplicationQuit()
    {
        OnLeaveChannel();
        videoChatModel.RtcEngine.Dispose();
        videoChatModel.RtcEngine = null;
    }
}