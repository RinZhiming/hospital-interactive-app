using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class PhotonChatView : MonoBehaviour
{
    private string currentUid;
    
    private void Awake()
    {
        chatInput.interactable = false;
        sendButton.interactable = false;
        
        PhotonChatEvent.OnConnectChat += OnConnectChat;
        PhotonChatEvent.OnMessageReceive += OnMessageReceive;
        
        sendButton.onClick.AddListener(SendButton);
    }

    private void Start()
    {
        ClearChild();
    }

    private void OnDestroy()
    {
        PhotonChatEvent.OnConnectChat -= OnConnectChat;
        PhotonChatEvent.OnMessageReceive -= OnMessageReceive;
    }

    private void OnConnectChat(string uid)
    {
        currentUid = uid;
        
        chatInput.interactable = true;
        sendButton.interactable = true;
        
        ClearChild();
    }

    private void OnMessageReceive(ChatSender authSender, string sender, object message)
    {
        var chatGameObject = Instantiate(authSender == ChatSender.Me ? myChatPrefab : friendChatPrefab, container);
        var chatObject = chatGameObject.GetComponent<AppointmentChatObject>();
        chatObject.ChatText.text = message as string;
    }
    
    private void SendButton()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            PhotonChatEvent.SendMessage?.Invoke(chatInput.text);
            chatInput.text = string.Empty;
            chatInput.Select();
        }
    }
    
    private void ClearChild()
    {
        if (!container) return;
        
        if (container.childCount <= 0) return;

        for (int i = 0; i < container.childCount; i++)
        {
            Destroy(container.GetChild(i).gameObject);
        }
    }
}
