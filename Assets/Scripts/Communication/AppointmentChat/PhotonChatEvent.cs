using System;
using Firebase.Auth;

public static class PhotonChatEvent
{
    public static Action<string> OnConnectChat;
    public static Action<ChatSender, string, object> OnMessageReceive;
    public static Action<string> SendMessage;
    public static Action OnDisconnect;
}