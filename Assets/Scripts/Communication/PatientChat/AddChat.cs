using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public partial class AddChat : MonoBehaviour
{
    private string currentPlayerId;
    private void Awake()
    {
        PlayerEvent.OnTargetPlayer += OnTargetChat;
        PlayerEvent.OnUnTargetPlayer += OnUnTargetChat;
        
        addChatButton.onClick.AddListener(AddChatButton);
    }

    private void Start()
    {
        addChatCanvasGroup.SetActive(false);
    }

    private void OnDestroy()
    {
        PlayerEvent.OnTargetPlayer -= OnTargetChat;
        PlayerEvent.OnUnTargetPlayer -= OnUnTargetChat;
    }
    
    private void AddChatButton()
    {
        if (!string.IsNullOrEmpty(currentPlayerId))
        {
            addChatCanvasGroup.gameObject.SetActive(false);
            ChatEvent.OnExitChat?.Invoke();
            ChatEvent.OnAddChatPlayer?.Invoke(currentPlayerId);
        }
    }

    private void OnTargetChat(object obj)
    {
        var playerGameObject = obj as GameObject;
        
        if (playerGameObject is null || AddChatEvent.IsAddChat || !ChatBoxManager.CanAddChat) return;
        
        var player = playerGameObject.GetComponent<NetworkPlayer>();

        if (player is null) return;

        if (string.IsNullOrEmpty(player.NetworkPlayerId.PlayerId)) return;
        currentPlayerId = player.NetworkPlayerId.PlayerId;
        foreach (var chatuser in ChatManager.ChatUsers)
        {
            if (chatuser.other == currentPlayerId)
            {
                addChatCanvasGroup.gameObject.SetActive(false);
                return;
            }
        }
        
        addChatCanvasGroup.SetActive(true);
    }
    
    private void OnUnTargetChat()
    {
        addChatCanvasGroup.SetActive(false);
        
        currentPlayerId = string.Empty;
    }
}
