using UnityEngine;
using UnityEngine.UI;

public class ChatObject : MonoBehaviour
{
    [SerializeField] private Text messageText, timetText;
    public ChatMessage Chat { get; set; }

    public Text MessageText
    {
        get => messageText;
        set => messageText = value;
    }

    public Text TimetText
    {
        get => timetText;
        set => timetText = value;
    }
}