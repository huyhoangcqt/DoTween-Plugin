using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Move",
        fileName = "MoveTween")]
    public class MoveTweenConfig : TweenConfigBase
    {
        [SerializeField]
        private Vector2 endPosition;

        [SerializeField]
        private bool relative;

        protected override Tween CreateTween(Component target)
        {
            if (target is not RectTransform rect)
            {
                Debug.LogWarning(
                    $"{name}: Target must be a RectTransform.", this);

                return null;
            }

            Tween tween = rect.DOAnchorPos(
                endPosition, Duration);

            return relative ? tween.SetRelative() : tween;
        }
    }
}