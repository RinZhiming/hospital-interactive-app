using System;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using InspectorEditor;
using UnityEngine;

public partial class ChatListManager : MonoBehaviour
{
    [SerializeField] private Sprite[] icons;
    private FirebaseAuth auth;
    private DatabaseReference dbRef;

    private void Awake()
    {
        ChatEvent.OnAddChatUser += AddChatList;
    }

    private void OnDestroy()
    {
        ClearChild();
        ChatEvent.OnAddChatUser -= AddChatList;
    }

    private void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
    }
    
    private void AddChatList()
    {
        ClearChild();
        foreach (var chat in ChatManager.ChatUsers)
        {
            AddNewChat(chat);
        }
    }

    private void AddNewChat(ChatUser chatusers)
    {
        dbRef.Child("users").Child(chatusers.other).Child("userdata").GetValueAsync().ContinueWithOnMainThread(result =>
        {
            if (result.IsFaulted || result.IsCanceled)
            {
                Debug.LogError(result.Exception.Message);
                return;
            }

            if (result.IsCompleted)
            {
                var rawdata = result.Result.GetRawJsonValue();
                var profile = JsonUtility.FromJson<UserData>(rawdata);

                var chatAvatar = Instantiate(chatListPrefab, contain);
                var chatAvatarObject = chatAvatar.GetComponent<ChatListAvatarObject>();
                chatAvatarObject.NameText.text = profile.username;
                chatAvatarObject.IconImage.sprite = icons[profile.iconProfile];
                chatAvatarObject.FriendData = profile;
                chatAvatarObject.ChatId = GetChatId(chatusers.owner, chatusers.other);
                chatAvatarObject.ChatUser = chatusers;
            }
        });
    }
    
    private string GetChatId(string user1, string user2)
    {
        string[] chatidarray = { user1, user2 };
        Array.Sort(chatidarray);
        var chatid = string.Join("&", chatidarray);
        
        return chatid;
    }
    
    private void ClearChild()
    {
        if (!contain) return;
        
        if (contain.childCount <= 0) return;
    
        for (int i = 0; i < contain.childCount; i++)
        {
            Destroy(contain.GetChild(i).gameObject);
        }
    }
}