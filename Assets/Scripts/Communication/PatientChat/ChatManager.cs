using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Fusion.Photon.Realtime;
using Photon.Chat;
using R3;
using UnityEngine;
using Utility;
using AuthenticationValues = Photon.Chat.AuthenticationValues;

public partial class ChatManager : MonoBehaviour, IChatClientListener
{
    private readonly Dictionary<string, List<ChatMessage>> chatDictionary = new ();
    private readonly List<ChatUser> chatUsers = new();
    private static ChatManager instance;
    private static readonly object lockObj = new();
    private bool isChatInit, isOnChat;
    private ChatClient chatClient;
    private FirebaseAuth auth;
    private ChatUser currentChatUser;
    private List<string> notifyChats = new List<string>();

    public static Dictionary<string, List<ChatMessage>> ChatDictionary => instance.chatDictionary;

    public static List<ChatUser> ChatUsers => instance.chatUsers;

    private void Awake()
    {
        if (!instance)
        {
            lock (lockObj)
            {
                if (!instance) instance = this;
            }
        }

        currentChatUser = null;

        ChatEvent.SendMessage += SendFriendMessage;
        
        chatButton.onClick.AddListener(ChatButton);
        closeButton.onClick.AddListener(CloseButton);
        backButton.onClick.AddListener(BackButton);

        chatList.alpha = 0;
        chatList.blocksRaycasts = false;
        chatBox.alpha = 0;
        chatBox.blocksRaycasts = false;
        chatMain.alpha = 0;
        chatMain.blocksRaycasts = false;

        chatButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        headerText.text = string.Empty;
        instance.iconImage.gameObject.SetActive(false);
        
        ChatListAvatarObject.OnClick += ChatListAvatarObjectOnClick;
        ChatEvent.CreateNewChat += CreateNewChat;

        chatClient = new ChatClient(this);
        notifyIcon.gameObject.SetActive(false);
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        
        chatClient.Connect(PhotonAppSettings.Global.AppSettings.AppIdChat,
            PhotonAppSettings.Global.AppSettings.AppVersion, new AuthenticationValues(auth.CurrentUser.UserId));
    }

    private void Update()
    {
        chatClient?.Service();
        
        notifyIcon.gameObject.SetActive(notifyChats.Count > 0);
    }

    private void OnDestroy()
    {
        ChatListAvatarObject.OnClick -= ChatListAvatarObjectOnClick;
        ChatEvent.CreateNewChat -= CreateNewChat;
        ChatEvent.SendMessage -= SendFriendMessage;
        instance = null;
    }
    
    private void ChatListAvatarObjectOnClick(ChatListAvatarObject obj)
    {
        chatList.SetActive(false);
        chatBox.SetActive(true);
        chatMain.SetActive(true);
        chatButton.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        
        currentChatUser = obj.ChatUser;
        headerText.text = obj.FriendData.username;
        iconImage.sprite = icons[obj.FriendData.iconProfile];
        isOnChat = true;
        RemoveNotify(obj.ChatUser.other);
    }

    private void CreateNewChat(string id, UserData profile)
    {
        var newChatUser = new ChatUser
        {
            owner = auth.CurrentUser.UserId,
            other = id,
        };
        Debug.Log(newChatUser.owner + " " + newChatUser.other);
        Debug.Log(id + " " + profile.username);
        currentChatUser = newChatUser;
        chatList.SetActive(false);
        chatBox.SetActive(true);
        chatMain.SetActive(true);
        chatButton.gameObject.SetActive(false);
        iconImage.gameObject.SetActive(true);
        backButton.gameObject.SetActive(true);
        
        headerText.text = profile.username;
        iconImage.sprite = icons[profile.iconProfile];
        isOnChat = true;
        RemoveNotify(id);
    }

    private void RemoveNotify(string id)
    {
        if (notifyChats.Contains(id))
        {
            notifyChats.Remove(id);
        }
    }
    private void ChatButton()
    {
        ChatEvent.OnAddChatUser?.Invoke();
        chatList.SetActive(true);
        chatMain.SetActive(true);
        chatButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        isOnChat = false;
    }

    private void CloseButton()
    {
        currentChatUser = null;
        
        chatList.SetActive(false);
        chatMain.SetActive(false);
        chatBox.SetActive(false);
        
        chatButton.gameObject.SetActive(true);
        iconImage.gameObject.SetActive(false);
        headerText.text = string.Empty;

        ChatEvent.OnExitChat?.Invoke();
        isOnChat = false;
    }

    private void BackButton()
    {
        currentChatUser = null;
        
        chatList.SetActive(true);
        chatMain.SetActive(true);
        chatBox.SetActive(false);
        
        iconImage.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        headerText.text = string.Empty;

        ChatEvent.OnExitChat?.Invoke();
        isOnChat = false;
        ChatEvent.OnAddChatUser?.Invoke();
    }

    private void SendFriendMessage(ChatMessage chat)
    {
        if (chatClient == null || currentChatUser == null || chat == null) return;
        chatClient.SendPrivateMessage(currentChatUser.other, chat.message);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected");
    }

    public void OnConnected()
    {
        Debug.Log("Connected");
        chatButton.gameObject.SetActive(true);
    }

    public void OnChatStateChange(ChatState state)
    {
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        var msgobj = new ChatMessage
        {
            sender = sender,
            message = message.ToString(),
            timestamp = DateTime.Now.ToString(),
        };
        var other = sender != auth.CurrentUser.UserId && currentChatUser == null ? sender : currentChatUser.other;
        var chatroom = GetChatId(auth.CurrentUser.UserId,other);
        
        if (chatDictionary.TryGetValue(chatroom, out var value))
        {
            value.Add(msgobj);
        }
        else
        {
            chatUsers.Add(new ChatUser { owner = auth.CurrentUser.UserId, other = other });
            chatDictionary.Add(chatroom, new List<ChatMessage> { msgobj });
        }
        
        if (currentChatUser != null)
        {
            ChatEvent.AddChatMessage(msgobj);
        }
        else
        {
            ChatEvent.OnAddChatUser?.Invoke();
        }
        
        if (sender != auth.CurrentUser.UserId)
        {
            if (!notifyChats.Contains(sender)) 
                notifyChats.Add(sender);
        }
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
    
    private string GetChatId(string user1, string user2)
    {
        string[] chatidarray = { user1, user2 };
        Array.Sort(chatidarray);
        var chatid = string.Join("&", chatidarray);

        return chatid;
    }
}

[SerializeField]
public class ChatUser
{
    public string owner;
    public string other;
}

[Serializable]
public class ChatMessage
{
    public string message;
    public string sender;
    public string timestamp;
}

[Serializable]
public class Chat : IEquatable<Chat>, IComparable<Chat>
{
    public string message;
    public string key;
    public string sender;
    public DateTime timestamp;

    public int CompareTo(Chat other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return timestamp.CompareTo(other.timestamp);
    }

    public bool Equals(Chat other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return message == other.message && sender == other.sender && key == other.key && timestamp.Equals(other.timestamp);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Chat)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(message, sender, key, timestamp);
    }
}