using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public partial class ChatBoxManager
{
    [SerializeField] private GameObject friendChatTextPrefab, myChatTextPrefab;
    [SerializeField] private GameObject chatButton;
    [SerializeField] private Button sendButton;
    [SerializeField] private TMP_InputField chatInput;
    [SerializeField] private Transform content;
    [SerializeField] private ScrollRect scroll;
}
