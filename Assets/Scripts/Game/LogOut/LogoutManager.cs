using Firebase.Auth;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class LogoutManager : MonoBehaviour
{
    [SerializeField] private string sceneLogin;

    private FirebaseAuth auth;

    private void Awake()
    {
        logoutButton.onClick.AddListener(Logout);
    }

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
    }

    private void Logout()
    {
        if (auth.CurrentUser != null) auth.SignOut();
        logoutPanel.SetActive(false);
        GameManager.Launcher.OnLeftRoom();
        SceneManager.LoadScene(sceneLogin);
    }
}
