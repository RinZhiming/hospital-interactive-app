using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerMinimapManager : MonoBehaviour
{
    [SerializeField] private Transform camera;
    public Transform Player { get; set; }

    private void Update()
    {
        if (PlayerManager.IsPause || NurseManager.IsDialogue) return;
        
        if (!Player || !camera) return;
        
        camera.transform.localPosition = new Vector3(
            Player.transform.localPosition.x, 
            camera.transform.localPosition.y,
            Player.transform.localPosition.z);
    }
}
