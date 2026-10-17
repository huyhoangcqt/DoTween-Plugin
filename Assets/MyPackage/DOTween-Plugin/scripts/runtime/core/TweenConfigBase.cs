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

        [SerializeField, Min(0f)]
        [Tooltip("Wait time between loops, in seconds.")]
        private float loopInterval;

        [SerializeField]
        private LoopType loopType = LoopType.Restart;

        [Header("Update")]
        [SerializeField]
        private bool ignoreTimeScale;

        public virtual float Duration => duration;
        public int Loops => loops;
        public float LoopInterval => loopInterval;

        // (Duration + LoopInterval) per loop; infinity when loops is -1.
        public float TotalDuration =>
            loops < 0 ? float.PositiveInfinity : (Duration + HoldInterval) * loops;

        private float HoldInterval => loops == 1 ? 0f : loopInterval;
        public float Delay => delay;
        public Ease Ease => ease;

        /// <summary>Raised whenever any config is modified (inspector edit or NotifyChanged).</summary>
        public static event System.Action<TweenConfigBase> AnyChanged;

        public void NotifyChanged() => AnyChanged?.Invoke(this);

        protected virtual void OnValidate()
        {
            if (Application.isPlaying)
                NotifyChanged();
        }

        /// <summary>True if this config is, or contains, the given config.</summary>
        public virtual bool Uses(TweenConfigBase other) => other == this;

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

            // DOTween has no loop interval, so one loop is wrapped together
            // with a trailing wait and the wrapper is what gets looped.
            if (HoldInterval > 0f)
            {
                tween.SetEase(ease);

                tween = DOTween.Sequence()
                    .Append(tween)
                    .AppendInterval(HoldInterval);

                return tween
                    .SetDelay(delay)
                    .SetLoops(loops, loopType)
                    .SetUpdate(UpdateType.Normal, ignoreTimeScale)
                    .SetAutoKill(false);
            }

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