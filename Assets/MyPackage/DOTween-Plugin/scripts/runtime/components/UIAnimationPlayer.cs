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

        public void Play()
        {
            Debug.Log("Play");
            DOTweenExecutor.Play(
                config,
                target != null ? target : transform,
                this);
        }

        public void Play(TweenConfigBase overrideConfig)
        {
            if (overrideConfig == null)
                return;

            DOTweenExecutor.Play(
                overrideConfig,
                target != null ? target : transform,
                this);
        }

        public void Stop()
        {
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