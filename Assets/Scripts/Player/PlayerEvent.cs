using System;
using Fusion;
using UnityEngine;

public static class PlayerEvent
{
    public static Action<object> OnTargetPlayer { get; set; }
    public static Action OnUnTargetPlayer { get; set; }
}