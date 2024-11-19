using Agora.Rtc;
using UnityEngine;
using UnityEngine.UI;

public partial class VideoChatView
{
    [SerializeField] private Button joinButton, leaveButton, muteMicrophoneButton, muteSpeakerButton;
    [SerializeField] private CanvasGroup videoChatCanvasGroup;
    [SerializeField] private RawImage patientCamImage, doctorCamImage;
    private VideoSurface patientVideo, doctorVideo;
}