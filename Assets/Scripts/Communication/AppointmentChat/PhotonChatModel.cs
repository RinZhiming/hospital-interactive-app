using Firebase.Auth;
using Firebase.Database;
using Fusion.Photon.Realtime;
using Photon.Chat;

public class PhotonChatModel
{
     public ChatClient ChatClient { get; set; }
     public FusionAppSettings Setting { get; set; }
     public FirebaseAuth Auth { get; set; }
     public DatabaseReference DatabaseReference { get; set; }
     private string ChannelName { get; set; }
}