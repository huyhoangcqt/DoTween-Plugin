using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Scale",
        fileName = "ScaleTween")]
    public class ScaleTweenConfig : TweenConfigBase
    {
        [SerializeField]
        private Vector3 endScale = Vector3.one;

        [SerializeField]
        private bool relative;

        protected override Tween CreateTween(Component target)
        {
            Tween tween = target.transform.DOScale(
                endScale, Duration);

            return relative ? tween.SetRelative() : tween;
        }
    }
}