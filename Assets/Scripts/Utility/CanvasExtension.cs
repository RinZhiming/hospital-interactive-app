using UnityEngine;

public static class CanvasExtension
{
    public static void SetActive(this CanvasGroup canvas, bool isShow)
    {
        canvas.alpha = isShow ? 1 : 0;
        canvas.interactable = isShow;
        canvas.blocksRaycasts = isShow;
    }

    public static bool IsActiveSelf(this CanvasGroup canvas)
    {
        return canvas.alpha != 0 || canvas.blocksRaycasts || canvas.interactable;
    }
}