using UnityEngine;

public static class CanvasGroupExtensions
{
    public static void SetAlpha(this CanvasGroup canvasGroup, float alpha)
    {
        if (canvasGroup != null)
            canvasGroup.alpha = alpha;
    }
}