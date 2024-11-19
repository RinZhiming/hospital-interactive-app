using System;
using System.Collections.Generic;

public static class ChatEvent
{
    public static Action OnAddChatUser { get; set; }
    public static Action OnExitChat { get; set; }
    public static Action<ChatMessage> SendMessage {get; set; }
    public static Action<ChatMessage> AddChatMessage { get; set; }
    public static Action<string> OnAddChatPlayer { get; set; }
    public static Action<string, UserData> CreateNewChat { get; set; }
    public static Action ChatNotify {get; set; }
}