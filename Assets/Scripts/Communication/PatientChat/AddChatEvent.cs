using System;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public partial class AddChatEvent : MonoBehaviour
{
    public static bool IsAddChat { get; private set; }
    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    
    private void Awake()
    {
        ChatEvent.OnAddChatPlayer += OnAddChatPlayer;
        ChatEvent.OnExitChat += OnExitChatPlayer;
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        IsAddChat = false;
    }

    private void OnDestroy()
    {
        ChatEvent.OnAddChatPlayer -= OnAddChatPlayer;
        ChatEvent.OnExitChat -= OnExitChatPlayer;
    }
    
    private void OnExitChatPlayer()
    {
        IsAddChat = false;
    }
    
    private void OnAddChatPlayer(string id)
    {
        
        GetPlayerProfileChat(id);
        IsAddChat = true;
    }

    private void GetPlayerProfileChat(string id)
    {
        dbRef.Child("users").Child(id).Child("userdata").GetValueAsync().ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                if (result.Exception != null) 
                    Debug.LogError(result.Exception.Message);
                return;
            }

            if (result.IsCompleted)
            {
                var rawdata = result.Result.GetRawJsonValue();
                var profile = JsonUtility.FromJson<UserData>(rawdata);
                
                SaveChat(id, profile);
            }
        });
    }

    private void SaveChat(string id, UserData profile)
    {
        CreateChatProfile(id, profile);
    }

    private void CreateChatProfile(string id, UserData profile)
    {
        var chatuser = new ChatUser
        {
            owner = auth.CurrentUser.UserId,
            other = id,
        };
        var chatRoomName = GetChatId(chatuser.owner, chatuser.other);
        ChatManager.ChatDictionary.TryAdd(chatRoomName, new List<ChatMessage>());
        if (!ChatManager.ChatUsers.Contains(chatuser)) ChatManager.ChatUsers.Add(chatuser);
        ChatEvent.CreateNewChat?.Invoke(id, profile);
        ChatEvent.OnAddChatUser?.Invoke();
    }
    
    private string GetChatId(string user1, string user2)
    {
        string[] chatidarray = { user1, user2 };
        Array.Sort(chatidarray);
        var chatid = string.Join("&", chatidarray);

        return chatid;
    }
}