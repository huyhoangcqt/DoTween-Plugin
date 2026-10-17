using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    public abstract class TweenConfigBase : ScriptableObject
    {
        [Header("Timing")]
        [SerializeField, Min(0f)]
        private float duration = 0.25f;

        [SerializeField, Min(0f)]
        private float delay;

        [SerializeField]
        private Ease ease = Ease.OutCubic;

        [Header("Loop")]
        [SerializeField, Min(-1)]
        private int loops = 1;

        [SerializeField]
        private LoopType loopType = LoopType.Restart;

        [Header("Update")]
        [SerializeField]
        private bool ignoreTimeScale;

        public float Duration => duration;
        public float Delay => delay;
        public Ease Ease => ease;

        public Tween BuildTween(Component target)
        {
            if (target == null)
            {
                Debug.LogWarning(
                    $"{name}: Tween target is null.", this);

                return null;
            }

            Tween tween = CreateTween(target);

            if (tween == null)
                return null;

            return tween
                .SetDelay(delay)
                .SetEase(ease)
                .SetLoops(loops, loopType)
                .SetUpdate(UpdateType.Normal, ignoreTimeScale)
                .SetAutoKill(false);
        }

        protected abstract Tween CreateTween(Component target);
    }
}