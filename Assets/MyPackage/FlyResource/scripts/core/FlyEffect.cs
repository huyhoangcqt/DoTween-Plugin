using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin.Fly
{
    /// <summary>
    /// Plays fly effects. Independent from TweenConfigBase / UIAnimationPlayer.
    /// Uses world positions so origin and destination may live in different canvases.
    /// </summary>
    public static class FlyEffect
    {
        public static Tween Play(
            FlyConfig config,
            FlyContext ctx,
            Action onArrive = null,
            FlyStyle style = FlyStyle.FlyCurved)
        {
            if (config == null || ctx.Target == null || ctx.Destination == null)
            {
                Debug.LogWarning("FlyEffect: config, Target and Destination are required.");
                return null;
            }

            Transform target = ctx.Target;
            Transform origin = ctx.Origin != null ? ctx.Origin : target;
            Transform destination = ctx.Destination;

            if (config.snapToOrigin && origin != target)
            {
                target.SetPositionAndRotation(origin.position, origin.rotation);
            }

            Vector3 originPos = target.position;
            Quaternion startRot = target.rotation;
            Vector3 startScale = target.localScale;
            Vector3 endScale = startScale * config.endScaleMultiplier;

            bool burstFirst =
                style == FlyStyle.FlyFirework || style == FlyStyle.FlyBlooming;

            // Fixed per item so the path is stable while the destination moves.
            Vector3 offset = Vector3.zero;
            Vector3 burstPoint = originPos;

            if (style == FlyStyle.FlyCurved)
            {
                offset = UnityEngine.Random.insideUnitSphere * config.spread;
                offset.z = 0f;
            }

            if (burstFirst)
            {
                Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;

                if (dir == Vector2.zero)
                    dir = Vector2.up;

                float radius = config.burstRadius
                    * UnityEngine.Random.Range(0.6f, 1f);

                burstPoint = originPos + (Vector3)(dir * radius);
            }

            Vector3 flyStart = burstPoint;
            float arc = style == FlyStyle.FlyStraight || burstFirst
                ? 0f
                : config.arcHeight;

            float burstT = 0f;
            float t = 0f;

            Sequence sequence = DOTween.Sequence()
                .SetDelay(config.delay)
                .SetUpdate(UpdateType.Normal, config.ignoreTimeScale)
                .SetLink(target.gameObject, LinkBehaviour.KillOnDestroy);

            if (burstFirst)
            {
                sequence.Append(DOTween.To(
                        () => burstT, x => burstT = x, 1f, config.burstDuration)
                    .SetEase(Ease.OutCubic)
                    .OnUpdate(() => target.position =
                        Vector3.LerpUnclamped(originPos, burstPoint, burstT)));

                if (style == FlyStyle.FlyBlooming && config.holdDuration > 0f)
                    sequence.AppendInterval(config.holdDuration);
            }

            sequence.Append(DOTween.To(() => t, x => t = x, 1f, config.duration)
                .SetEase(config.ease)
                .OnUpdate(() =>
                {
                    if (destination == null)
                        return;

                    Vector3 end = destination.position;
                    Vector3 control = (flyStart + end) * 0.5f
                        + Vector3.up * arc + offset;

                    float u = 1f - t;
                    target.position = u * u * flyStart
                        + 2f * u * t * control
                        + t * t * end;

                    target.localScale = Vector3.LerpUnclamped(
                        startScale, endScale, t);

                    if (config.matchDestinationRotation)
                        target.rotation = Quaternion.SlerpUnclamped(
                            startRot, destination.rotation, t);
                }));

            sequence.OnComplete(() =>
            {
                if (config.deactivateOnArrive)
                    target.gameObject.SetActive(false);

                onArrive?.Invoke();
            });

            return sequence;
        }

        /// <summary>
        /// Plays several items one after another with the config's gap interval.
        /// </summary>
        public static Sequence PlayBurst(
            FlyConfig config,
            IReadOnlyList<FlyContext> items,
            Action<int> onEachArrive = null,
            Action onAllArrive = null,
            FlyStyle style = FlyStyle.FlyCurved)
        {
            if (config == null || items == null || items.Count == 0)
                return null;

            Sequence sequence = DOTween.Sequence()
                .SetUpdate(UpdateType.Normal, config.ignoreTimeScale);

            for (int i = 0; i < items.Count; i++)
            {
                int index = i;

                Tween tween = Play(
                    config, items[i], () => onEachArrive?.Invoke(index), style);

                if (tween != null)
                    sequence.Insert(i * config.gapInterval, tween);
            }

            if (onAllArrive != null)
                sequence.OnComplete(() => onAllArrive());

            return sequence;
        }

        /// <summary>
        /// Splits a reward into coins: min(added, maxCoin) coins whose values
        /// sum to added. E.g. 28 with max 10 -> 8 coins of 3 and 2 coins of 2.
        /// </summary>
        public static int[] SplitValue(int added, int maxCoin)
        {
            if (added <= 0)
                return Array.Empty<int>();

            int count = Mathf.Min(added, Mathf.Max(1, maxCoin));
            int baseValue = added / count;
            int remainder = added % count;

            var values = new int[count];

            for (int i = 0; i < count; i++)
                values[i] = i < remainder ? baseValue + 1 : baseValue;

            return values;
        }

        /// <summary>
        /// Flies coins from origin to the target to give a currency reward.
        /// The displayed value counts up from currentValue as each coin
        /// arrives and ends exactly at afterValue.
        /// </summary>
        /// <param name="getCoin">Returns an active coin object for each item.</param>
        /// <param name="onValueChanged">Called with the displayed value after each coin.</param>
        public static Sequence PlayCurrency(
            FlyConfig config,
            int currentValue,
            int afterValue,
            Transform origin,
            FlyTarget target,
            Func<Transform> getCoin,
            Action<int> onValueChanged = null,
            Action onAllArrive = null,
            FlyStyle style = FlyStyle.FlyCurved)
        {
            if (config == null || target == null || getCoin == null)
                return null;

            int[] values = SplitValue(afterValue - currentValue, config.maxCoin);

            if (values.Length == 0)
            {
                target.SetValue(afterValue);
                onValueChanged?.Invoke(afterValue);
                onAllArrive?.Invoke();
                return null;
            }

            var items = new List<FlyContext>(values.Length);

            foreach (int _ in values)
                items.Add(new FlyContext(getCoin(), origin, target.Icon));

            int shown = currentValue;
            target.SetValue(shown);

            return PlayBurst(
                config,
                items,
                index =>
                {
                    shown += values[index];
                    target.SetValue(shown);
                    target.Punch();
                    onValueChanged?.Invoke(shown);
                },
                onAllArrive,
                style);
        }
    }
}
