using System.Collections;
using System.Collections.Generic;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;

public class AppointmentModel : MonoBehaviour
{
    public Appointment Appointment { get; set; }
    public FirebaseAuth Auth { get; set; }
    public DatabaseReference DatabaseReference { get; set; }
}
