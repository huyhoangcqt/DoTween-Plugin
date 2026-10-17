using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    public static class DOTweenExecutor
    {
        private static readonly Dictionary<int, Tween> ActiveTweens =
            new Dictionary<int, Tween>();

        public static bool Play(
            TweenConfigBase config,
            Component target,
            MonoBehaviour owner,
            bool rewindPrevious = false)
        {
            if (config == null || target == null || owner == null)
                return false;

            if (rewindPrevious && TryGetTween(owner, out Tween previous))
                previous.Rewind();

            Stop(owner);

            Tween tween = config.BuildTween(target);

            if (tween == null)
                return false;

            int ownerId = owner.GetInstanceID();

            tween.SetLink(
                owner.gameObject,
                LinkBehaviour.KillOnDestroy);

            tween.OnKill(() =>
            {
                if (ActiveTweens.TryGetValue(
                    ownerId, out Tween current) &&
                    current == tween)
                {
                    ActiveTweens.Remove(ownerId);
                }
            });

            ActiveTweens[ownerId] = tween;
            tween.Play();

            return true;
        }

        public static void Stop(MonoBehaviour owner)
        {
            if (owner == null)
                return;

            int id = owner.GetInstanceID();

            if (ActiveTweens.TryGetValue(id, out Tween tween))
            {
                ActiveTweens.Remove(id);

                if (tween != null && tween.IsActive())
                    tween.Kill();
            }
        }

        public static void Complete(MonoBehaviour owner)
        {
            if (TryGetTween(owner, out Tween tween))
                tween.Complete();
        }

        public static bool Restart(MonoBehaviour owner)
        {
            if (!TryGetTween(owner, out Tween tween))
                return false;

            tween.Restart();
            return true;
        }

        private static bool TryGetTween(
            MonoBehaviour owner,
            out     Tween tween)
        {
            tween = null;

            if (owner == null)
                return false;

            return ActiveTweens.TryGetValue(
                owner.GetInstanceID(), out tween)
                && tween != null
                && tween.IsActive();
        }
    }
}