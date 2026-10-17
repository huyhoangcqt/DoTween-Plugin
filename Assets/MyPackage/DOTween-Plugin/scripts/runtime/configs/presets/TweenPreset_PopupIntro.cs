using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Preset/Popup Intro",
        fileName = "PopupIntro")]
    public class TweenPreset_PopupIntro : TweenConfigPreset
    {
        [Header("Scale")]
        [SerializeField, Min(0f)]
        private float scaleDuration = 0.35f;

        [SerializeField]
        private Vector3 startScale = new Vector3(0.6f, 0.6f, 1f);

        [SerializeField]
        private Vector3 endScale = Vector3.one;

        [SerializeField]
        private Ease scaleEase = Ease.OutBack;

        [SerializeField, Min(0f)]
        private float overshoot = 1.7f;

        [Header("Fade")]
        [SerializeField, Min(0f)]
        private float fadeDuration = 0.2f;

        [SerializeField, Range(0f, 1f)]
        private float startAlpha;

        [SerializeField, Range(0f, 1f)]
        private float endAlpha = 1f;

        public override float Duration =>
            Mathf.Max(scaleDuration, fadeDuration);

        protected override Tween CreateTween(Component target)
        {
            CanvasGroup group = GetOrAddCanvasGroup(target);

            Sequence sequence = DOTween.Sequence();

            sequence.Insert(0f, target.transform
                .DOScale(endScale, scaleDuration)
                .From(startScale)
                .SetEase(scaleEase, overshoot));

            sequence.Insert(0f, group
                .DOFade(endAlpha, fadeDuration)
                .From(startAlpha)
                .SetEase(Ease.OutQuad));

            return sequence;
        }
    }
}
