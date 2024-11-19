using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class PhotonChatView
{
    [SerializeField] private InputField chatInput;
    [SerializeField] private Button sendButton;
    [SerializeField] private GameObject myChatPrefab, friendChatPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private ScrollRect scroll;
}
