using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AppointmentPlayer : MonoBehaviour
{
    [SerializeField] private Animator animator;
    public void Start()
    {
        animator.SetBool("Grounded", true);
        animator.SetFloat("MoveSpeed", 0);
    }
}