using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Preset/Popup Outro",
        fileName = "PopupOutro")]
    public class TweenPreset_PopupOutro : TweenConfigPreset
    {
        [Header("Scale")]
        [SerializeField, Min(0f)]
        private float scaleDuration = 0.25f;

        [SerializeField]
        private Vector3 endScale = new Vector3(0.6f, 0.6f, 1f);

        [SerializeField]
        private Ease scaleEase = Ease.InBack;

        [SerializeField, Min(0f)]
        private float overshoot = 1.7f;

        [Header("Fade")]
        [SerializeField, Min(0f)]
        private float fadeDuration = 0.2f;

        [SerializeField, Range(0f, 1f)]
        private float endAlpha;

        [Header("After")]
        [SerializeField]
        private bool deactivateOnComplete;

        public override float Duration =>
            Mathf.Max(scaleDuration, fadeDuration);

        protected override Tween CreateTween(Component target)
        {
            CanvasGroup group = GetOrAddCanvasGroup(target);

            Sequence sequence = DOTween.Sequence();

            sequence.Insert(0f, target.transform
                .DOScale(endScale, scaleDuration)
                .SetEase(scaleEase, overshoot));

            sequence.Insert(0f, group
                .DOFade(endAlpha, fadeDuration)
                .SetEase(Ease.InQuad));

            if (deactivateOnComplete)
            {
                GameObject go = target.gameObject;
                sequence.OnComplete(() => go.SetActive(false));
            }

            return sequence;
        }
    }
}
