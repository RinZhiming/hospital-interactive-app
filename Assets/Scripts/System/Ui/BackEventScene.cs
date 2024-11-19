using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackEventScene : MonoBehaviour
{
    [SerializeField] private string targetScene;
    private Button button;

    private void Awake()
    {
        button = GetComponentInParent<Button>();
    }

    private void Start()
    {
        if (button != null) button.onClick.AddListener(Event);
    }

    private void Event()
    {
        SceneManager.LoadScene(targetScene);
    }
}
