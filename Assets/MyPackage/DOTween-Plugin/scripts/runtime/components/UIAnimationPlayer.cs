using System.Collections.Generic;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    public class UIAnimationPlayer : MonoBehaviour
    {
        [SerializeField]
        private TweenConfigBase config;

        [SerializeField]
        private RectTransform target;

        public TweenConfigBase Config
        {
            get => config;
            set => config = value;
        }

        public RectTransform Target
        {
            get => target;
            set => target = value;
        }

        [SerializeField]
        private bool liveReload = true;

        [Header("Recorded States")]
        [SerializeField]
        private TweenObjectState defaultState;

        [SerializeField]
        private List<TweenObjectState> states = new List<TweenObjectState>();

        public TweenObjectState DefaultState
        {
            get => defaultState;
            set => defaultState = value;
        }

        public List<TweenObjectState> States => states;

        public Transform TargetTransform => ResolveTarget();

        private TweenConfigBase playedConfig;

        private Transform initialTransform;
        private Vector3 initialLocalPosition;
        private Quaternion initialLocalRotation;
        private Vector3 initialLocalScale;
        private Vector2 initialAnchoredPosition;
        private Vector2 initialSizeDelta;
        private CanvasGroup initialCanvasGroup;
        private float initialAlpha;

        private void Awake()
        {
            CaptureInitialState();
        }

        private Transform ResolveTarget()
        {
            return target != null ? target : transform;
        }

        public void CaptureInitialState()
        {
            Transform t = ResolveTarget();
            initialTransform = t;
            initialLocalPosition = t.localPosition;
            initialLocalRotation = t.localRotation;
            initialLocalScale = t.localScale;

            if (t is RectTransform rect)
            {
                initialAnchoredPosition = rect.anchoredPosition;
                initialSizeDelta = rect.sizeDelta;
            }

            initialCanvasGroup = t.GetComponent<CanvasGroup>();

            if (initialCanvasGroup != null)
                initialAlpha = initialCanvasGroup.alpha;
        }

        /// <summary>Stops the tween and restores the target to its state at Awake.</summary>
        public void ResetState()
        {
            Stop();

            if (initialTransform == null)
                return;

            initialTransform.localPosition = initialLocalPosition;
            initialTransform.localRotation = initialLocalRotation;
            initialTransform.localScale = initialLocalScale;

            if (initialTransform is RectTransform rect)
            {
                rect.anchoredPosition = initialAnchoredPosition;
                rect.sizeDelta = initialSizeDelta;
            }

            if (initialCanvasGroup != null)
                initialCanvasGroup.alpha = initialAlpha;
            else if (initialTransform.TryGetComponent(out CanvasGroup added))
                added.alpha = 1f; // CanvasGroup was added by a fade tween.
        }

        private void OnEnable()
        {
            TweenConfigBase.AnyChanged += OnAnyConfigChanged;
        }

        private void OnDisable()
        {
            TweenConfigBase.AnyChanged -= OnAnyConfigChanged;
        }

        // Rebuilds the running tween so edits to the config apply immediately.
        private void OnAnyConfigChanged(TweenConfigBase changed)
        {
            if (!liveReload || playedConfig == null || changed == null)
                return;

            if (!playedConfig.Uses(changed))
                return;

            DOTweenExecutor.Play(
                playedConfig,
                target != null ? target : transform,
                this,
                true);
        }

        public void Play()
        {
            PlayInternal(config);
        }

        public void Play(TweenConfigBase overrideConfig)
        {
            if (overrideConfig == null)
                return;

            PlayInternal(overrideConfig);
        }

        private void PlayInternal(TweenConfigBase toPlay)
        {
            if (DOTweenExecutor.Play(
                toPlay,
                target != null ? target : transform,
                this))
            {
                playedConfig = toPlay;
            }
        }

        public void Stop()
        {
            playedConfig = null;
            DOTweenExecutor.Stop(this);
        }

        public void Complete()
        {
            DOTweenExecutor.Complete(this);
        }

        public void Restart()
        {
            if (!DOTweenExecutor.Restart(this))
                Play();
        }

        private void OnDestroy()
        {
            DOTweenExecutor.Stop(this);
        }
    }
}