using UnityEngine;
using UnityEngine.UI;

public class AppointmentChatObject : MonoBehaviour
{
    [SerializeField] private Text chatText;

    public Text ChatText
    {
        get => chatText;
        set => chatText = value;
    }
}