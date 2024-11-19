using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class CharacterSelectManager
{
    [SerializeField] private Transform contain;
    [SerializeField] private Button backButton, nextButton, selectButton;
    [SerializeField] private float scrollSpeed = 0.3f;
    private bool onSlide = false;
    public static int CharacterIndex { get; set; } = -1;
}