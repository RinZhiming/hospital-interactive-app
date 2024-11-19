using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatListAvatarObject : MonoBehaviour
{
    [SerializeField] private Text nameText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button button;
    public static event Action<ChatListAvatarObject> OnClick; 
    public string ChatId { get; set; }
    public UserData FriendData { get; set; }
    public ChatUser ChatUser { get; set; }
    
    private void Awake()
    {
        button.onClick.AddListener(OnClickButton);
    }

    private void OnClickButton()
    {
        OnClick?.Invoke(this);
    }

    public Text NameText
    {
        get => nameText;
        set => nameText = value;
    }

    public Image IconImage
    {
        get => iconImage;
        set => iconImage = value;
    }
}