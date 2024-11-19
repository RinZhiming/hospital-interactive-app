using System;
using Firebase.Auth;
using Firebase.Database;
using System.Collections.Generic;
using Firebase.Extensions;
using UnityEngine;

public partial class ChatBoxManager : MonoBehaviour
{
    private List<ChatMessage> currentChatsList = new();
    private FirebaseAuth auth;
    private DatabaseReference dbRef;
    private static ChatBoxManager instance;
    private string currentChat;
    private string usernameCatch;
    private int endMessageIndex;
    public static bool CanAddChat { get; private set; }

    private void Awake()
    {
        ChatListAvatarObject.OnClick += ChatListAvatarObjectOnClick;
        ChatEvent.OnExitChat += OnExitChat;
        ChatEvent.CreateNewChat += OnAddChat;
        ChatEvent.AddChatMessage += AddChatMessage;
        
        sendButton.onClick.AddListener(SendButton);
        currentChatsList.Clear();

        if (chatInput) chatInput.interactable = true;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    private void OnDestroy()
    {
        ClearChild();
        
        ChatListAvatarObject.OnClick -= ChatListAvatarObjectOnClick;
        ChatEvent.OnExitChat -= OnExitChat;
        ChatEvent.CreateNewChat -= OnAddChat;
        ChatEvent.AddChatMessage -= AddChatMessage;
    }
    
    private void Start()
    {
        dbRef = FirebaseDatabase.DefaultInstance.RootReference;
        auth = FirebaseAuth.DefaultInstance;
        CanAddChat = true;
        
        currentChat = string.Empty;
    }
    
    private void OnExitChat()
    {
        ClearChild();
        currentChatsList.Clear();
        currentChat = string.Empty;
    }

    private void ChatListAvatarObjectOnClick(ChatListAvatarObject obj)
    {
        Debug.Log($"On Initialize ChatListAvatarObjectOnClick : {obj.ChatId}");
        if (chatInput) 
            chatInput.interactable = true;
        
        currentChat = obj.ChatId;
        
        AddInitChatMessage();
        Canvas.ForceUpdateCanvases();
        if (scroll) scroll.verticalNormalizedPosition = 0;
    }

    private void AddInitChatMessage()
    {
        ClearChild();
        
        if (string.IsNullOrEmpty(currentChat)) return;
        
        if (ChatManager.ChatDictionary.TryGetValue(currentChat, out var chatlist))
        {
            foreach (var chat in chatlist)
            {
                CreateChat(chat);
                currentChatsList.Add(chat);
            }
        }

        Canvas.ForceUpdateCanvases();
        if (scroll)
        {
            scroll.CalculateLayoutInputVertical();
            scroll.SetLayoutVertical();
            scroll.verticalNormalizedPosition = 0;
        }
    }

    private void AddChatMessage(ChatMessage chat)
    {
        CreateChat(chat);
        currentChatsList.Add(chat);
    }

    private void CreateChat(ChatMessage chat)
    {
        var chatGameobject = 
            Instantiate(chat.sender == auth.CurrentUser.UserId ? myChatTextPrefab : friendChatTextPrefab, content);
            
        var chatObject = chatGameobject.GetComponent<ChatObject>();
            
        chatObject.MessageText.text = chat.message;
        chatObject.TimetText.text = $"{chat.timestamp}";
        chatObject.Chat = chat;
        
        chatGameobject.transform.SetAsLastSibling();
        Canvas.ForceUpdateCanvases();
        if (scroll)
        {
            scroll.CalculateLayoutInputVertical();
            scroll.SetLayoutVertical();
            scroll.verticalNormalizedPosition = 0;
        }
    }

    private void OnAddChat(string id, UserData profile)
    {
        currentChat = id;
    }
    
    private void SendButton()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
            SendNewChat();
    }

    private void SendNewChat()
    {
        SendChatMessage(chatInput.text, auth.CurrentUser.UserId);
    }

    private void SendChatMessage(string message, string sender)
    {
        if (string.IsNullOrEmpty(currentChat)) 
            return;
        
        var newmessageraw = new ChatMessage
        {
            message = message,
            sender = sender,
            timestamp = DateTime.Now.ToString()
        };
        
        SendChatMessage(newmessageraw);
    }

    private void SendChatMessage(ChatMessage chat)
    {
        ChatEvent.SendMessage?.Invoke(chat);
        chatInput.text = string.Empty;
    }

    private void ClearChild()
    {
        if (!content) return;
        
        if (content.childCount <= 0) return;

        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    private string GetChatId(string user1, string user2)
    {
        string[] chatidarray = { user1, user2 };
        Array.Sort(chatidarray);
        var chatid = string.Join("&", chatidarray);

        return chatid;
    }
}