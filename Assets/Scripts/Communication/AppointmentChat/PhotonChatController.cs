using System;
using ExitGames.Client.Photon;
using Firebase.Auth;
using Firebase.Database;
using Fusion.Photon.Realtime;
using Photon.Chat;
using UnityEngine;
using AuthenticationValues = Photon.Chat.AuthenticationValues;

public enum ChatSender
{
    Me,
    Other
}
public class PhotonChatController : MonoBehaviour, IChatClientListener
{
    private PhotonChatModel model;
    private AppointmentClone appointment;
    
    private void Awake()
    {
        PhotonChatEvent.SendMessage += SendPrivateMessage;
        PhotonChatEvent.OnDisconnect += Disconnect;
        model = new();
    }

    private void OnApplicationQuit()
    {
        PhotonChatEvent.OnDisconnect?.Invoke();
    }

    private void OnDestroy()
    {
        PhotonChatEvent.SendMessage -= SendPrivateMessage;
        PhotonChatEvent.OnDisconnect -= Disconnect;
    }

    private void Start()
    {
        appointment = NurseSeeDoctorManager.CurrentAppointment;
        
        model.Auth = FirebaseAuth.DefaultInstance;
        model.DatabaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        model.Setting = PhotonAppSettings.Global.AppSettings;

        model.ChatClient = new ChatClient(this);
        model.ChatClient.Connect(model.Setting.AppIdChat, model.Setting.AppVersion, new AuthenticationValues(model.Auth.CurrentUser.UserId));
    }

    private void Update()
    {
        if (model.ChatClient != null)
        {
            model.ChatClient.Service();
        }
    }

    private void Disconnect()
    {
        SendPrivateMessage("<b>Goodbye... Patient Left Room</b>");
        model.ChatClient.Disconnect();
    }

    public void DebugReturn(DebugLevel level, string message)
    {
    }

    public void OnDisconnected()
    {
        model.ChatClient.RemoveFriends(new [] { appointment.doctorId });
    }

    public void OnConnected()
    {
        PhotonChatEvent.OnConnectChat?.Invoke(model.Auth.CurrentUser.UserId);
        Debug.Log("Chat Connected");
        model.ChatClient.AddFriends(new [] { appointment.doctorId });
        SendPrivateMessage("<b>Hi! Patient Enter Room</b>");
    }

    private void SendPrivateMessage(string message)
    {
        model.ChatClient.SendPrivateMessage(appointment.doctorId, message);
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        PhotonChatEvent.OnMessageReceive?.Invoke(
            model.Auth.CurrentUser.UserId == sender ? ChatSender.Me : ChatSender.Other ,
            sender, 
            message);
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
    }

    public void OnUnsubscribed(string[] channels)
    {
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
    }

    public void OnUserSubscribed(string channel, string user)
    {
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
    }
}
