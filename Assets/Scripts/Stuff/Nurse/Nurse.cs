using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Nurse : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator nurseAnimator;
    [SerializeField] private NurseTrigger trigger;
    [SerializeField] private Transform nurseWalkPoint, door;
    [SerializeField] private Vector3 openPos;
    [SerializeField] private CinemachineVirtualCamera nurseBlendCamera;
    private float distance;
    private void Start()
    {
        nurseAnimator.SetBool("Grounded", true);
        nurseAnimator.SetFloat("MoveSpeed", 0);
    }

    private void Update()
    {
        distance = Vector3.Distance(transform.position, nurseWalkPoint.position);
    }

    public void WalkToPoint()
    {
        agent.SetDestination(nurseWalkPoint.position);

        StartCoroutine(WalkToPointDelay());
    }

    private IEnumerator WalkToPointDelay()
    {
        nurseAnimator.SetFloat("MoveSpeed", 1);
        yield return new WaitUntil(() => distance < 0.5f);
        nurseAnimator.SetFloat("MoveSpeed", 0);
        door.DORotate(openPos, 0.5f).SetEase(Ease.InOutSine);
    }

    public CinemachineVirtualCamera NurseBlendCamera
    {
        get => nurseBlendCamera;
        set => nurseBlendCamera = value;
    }
}
