using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public partial class ChatManager
{
    [SerializeField] private CanvasGroup chatList, chatBox, chatMain;
    [SerializeField] private Button chatButton, closeButton, backButton;
    [SerializeField] private Text headerText;
    [SerializeField] private Image iconImage, notifyIcon;
    [SerializeField] private Sprite[] icons;
}