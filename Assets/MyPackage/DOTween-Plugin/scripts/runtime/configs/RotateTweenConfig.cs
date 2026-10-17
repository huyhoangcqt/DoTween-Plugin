using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Rotate",
        fileName = "RotateTween")]
    public class RotateTweenConfig : TweenConfigBase
    {
        [SerializeField]
        private Vector3 endRotation;

        [SerializeField]
        private bool relative;

        protected override Tween CreateTween(Component target)
        {
            Tween tween = target.transform.DOLocalRotate(
                endRotation, Duration);

            return relative ? tween.SetRelative() : tween;
        }
    }
}