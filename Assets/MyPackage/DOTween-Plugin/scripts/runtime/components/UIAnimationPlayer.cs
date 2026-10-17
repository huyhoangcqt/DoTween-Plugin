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

        private TweenConfigBase playedConfig;

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