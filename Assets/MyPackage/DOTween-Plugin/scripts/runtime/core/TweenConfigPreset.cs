using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    /// <summary>
    /// A ready-made animation built in code from a few parameters.
    /// It behaves like a Sequence: eases are set per tween by the preset,
    /// and Duration is calculated from its own parameters.
    /// </summary>
    public abstract class TweenConfigPreset : TweenConfigBase
    {
        // Each preset picks its own ease per tween, so the shared ease is unused.
        protected override void ApplyEase(Tween tween)
        {
        }

        protected static CanvasGroup GetOrAddCanvasGroup(Component target)
        {
            if (!target.TryGetComponent(out CanvasGroup group))
                group = target.gameObject.AddComponent<CanvasGroup>();

            return group;
        }
    }
}
