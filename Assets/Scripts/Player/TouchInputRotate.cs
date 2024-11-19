using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputRotate : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private PlayerManager playerManager;
    private bool isPress;
    private int inputId;
    private Vector2 inputPosition, inputDistance;

    public Vector2 InputDistance
    {
        get => inputDistance;
        set => inputDistance = value;
    }

    private void Update()
    {
        if (PlayerManager.IsPause || NurseManager.IsDialogue) return;
        
        InputRotate();
    }

    private void InputRotate()
    {
        if (isPress)
        {
            if (inputId >= 0 && inputId < Input.touches.Length)
            {
                inputDistance = Input.touches[inputId].position - inputPosition;
                inputPosition = Input.touches[inputId].position;
            }
            else
            {
                inputDistance = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - inputPosition;
                inputPosition = Input.mousePosition;
            }
        }
        else
        {
            inputDistance = new Vector2();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPress = true;
        inputId = eventData.pointerId;
        inputPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPress = false;
    }

}
