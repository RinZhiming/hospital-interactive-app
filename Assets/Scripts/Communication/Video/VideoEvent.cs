using System;
using Agora.Rtc;

public static class VideoEvent
{
    public static Action<string> OnJoinChannel;
    public static Action OnLeaveChannel;
    public static Action OnJoinChannelSuccessful;
    public static Action OnLeaveChannelSuccessful;
    public static Action OnInitSuccessful;
    public static Action OnError;
    public static Action<uint,string,VIDEO_SOURCE_TYPE> RemoteUserJoin;
    public static Action LocalUserJoin;
    public static Action RemoteUserLeave;
    public static Action MuteSpeaker;
    public static Action MuteMicrophone;
}