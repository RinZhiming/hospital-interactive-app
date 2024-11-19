using System;
using System.Collections;
using System.Collections.Generic;
using Agora.Rtc;
using UnityEngine;

public class VideoChatModel
{
    public IRtcEngine RtcEngine { get; set; }
}

internal class VideoChatEvent : IRtcEngineEventHandler
{
    private readonly VideoChatController videoChat;

    public VideoChatEvent(VideoChatController videoChat)
    {
        this.videoChat = videoChat;
    }

    public override void OnUserJoined(RtcConnection connection, uint remoteUid, int elapsed)
    {
        VideoEvent.RemoteUserJoin?.Invoke(remoteUid, connection.channelId, VIDEO_SOURCE_TYPE.VIDEO_SOURCE_REMOTE);
    }

    public override void OnUserOffline(RtcConnection connection, uint remoteUid, USER_OFFLINE_REASON_TYPE reason)
    {
        VideoEvent.RemoteUserLeave?.Invoke();
    }

    public override void OnJoinChannelSuccess(RtcConnection connection, int elapsed)
    {
        VideoEvent.LocalUserJoin?.Invoke();
        VideoEvent.OnJoinChannelSuccessful?.Invoke();
        Debug.Log("Agora: OnJoinChannelSuccess : " + connection.channelId);
    }

    public override void OnLeaveChannel(RtcConnection connection, RtcStats stats)
    {
        VideoEvent.OnLeaveChannelSuccessful?.Invoke();
        Debug.Log("Agora: OnLeaveChannelSuccess : " + connection.channelId);
    }

    public override void OnError(int err, string msg)
    {
        VideoEvent.OnError?.Invoke();
        Debug.Log(err + msg);
    }
}