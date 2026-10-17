using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Fade",
        fileName = "FadeTween")]
    public class FadeTweenConfig : TweenConfigBase
    {
        [SerializeField, Range(0f, 1f)]
        private float endAlpha = 1f;

        protected override Tween CreateTween(Component target)
        {
            CanvasGroup canvasGroup =
                target as CanvasGroup ??
                target.GetComponent<CanvasGroup>();

            if (canvasGroup == null)
            {
                Debug.LogWarning(
                    $"{name}: Target requires a CanvasGroup.", this);

                return null;
            }

            return canvasGroup.DOFade(endAlpha, Duration);
        }
    }
}