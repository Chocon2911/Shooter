using UnityEngine;

public static class TransformExtensions
{
    public static void CopyFrom(this Transform target, Transform source)
    {
        if (target == null || source == null) return;
        target.position = source.position;
        target.rotation = source.rotation;
        target.localScale = source.localScale;
    }

    public static void SafeSetParent(this Transform child, Transform newParent)
    {
        if (child == null) return;
        Vector3 originalLocalScale = child.localScale;
        child.SetParent(newParent, false);
        child.localScale = originalLocalScale;
    }

    public static void SafeSetParent(this RectTransform child, Transform newParent)
    {
        if (child == null) return;

        Vector3 worldPos = child.position;
        Quaternion worldRot = child.rotation;
        Vector3 localScale = child.localScale;

        Vector2 anchorMin = child.anchorMin;
        Vector2 anchorMax = child.anchorMax;
        Vector2 anchoredPosition = child.anchoredPosition;
        Vector2 sizeDelta = child.sizeDelta;
        Vector2 pivot = child.pivot;

        child.SetParent(newParent, false);

        child.position = worldPos;
        child.rotation = worldRot;
        child.localScale = localScale;

        child.anchorMin = anchorMin;
        child.anchorMax = anchorMax;
        child.anchoredPosition = anchoredPosition;
        child.sizeDelta = sizeDelta;
        child.pivot = pivot;
    }
}