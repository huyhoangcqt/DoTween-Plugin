using UnityEngine;

namespace YellowCat.TweenPlugin
{
    /// <summary>Snapshot of a target's position, alpha, scale and rotation.</summary>
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Object State",
        fileName = "ObjectState")]
    public class TweenObjectState : ScriptableObject
    {
        public Vector2 position;
        [Range(0f, 1f)]
        public float alpha = 1f;
        public Vector3 scale = Vector3.one;
        public Vector3 rotation;

        public void Capture(Transform target)
        {
            position = target is RectTransform rect
                ? rect.anchoredPosition
                : (Vector2)target.localPosition;

            scale = target.localScale;
            rotation = target.localEulerAngles;

            alpha = target.TryGetComponent(out CanvasGroup group)
                ? group.alpha
                : 1f;
        }

        public void Apply(Transform target)
        {
            if (target is RectTransform rect)
                rect.anchoredPosition = position;
            else
                target.localPosition = new Vector3(
                    position.x, position.y, target.localPosition.z);

            target.localScale = scale;
            target.localEulerAngles = rotation;

            if (target.TryGetComponent(out CanvasGroup group))
                group.alpha = alpha;
        }
    }
}
