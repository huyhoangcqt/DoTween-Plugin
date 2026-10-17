using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin
{
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Sequence",
        fileName = "SequenceTween")]
    public class SequenceTweenConfig : TweenConfigBase
    {
        public enum JoinMode
        {
            Append,
            Join,
        }

        [Serializable]
        public class Step
        {
            public TweenConfigBase config;
            public JoinMode mode = JoinMode.Append;

            [Min(0f)]
            [Tooltip("Extra gap inserted before this step (Append only).")]
            public float interval;
        }

        [SerializeField]
        private List<Step> steps = new List<Step>();

        public IReadOnlyList<Step> Steps => steps;

        public override bool Uses(TweenConfigBase other)
        {
            return Uses(other, 0);
        }

        private bool Uses(TweenConfigBase other, int depth)
        {
            if (other == this)
                return true;

            if (depth > 16)
                return false;

            foreach (Step step in steps)
            {
                if (step == null || step.config == null)
                    continue;

                if (step.config is SequenceTweenConfig seq)
                {
                    if (seq.Uses(other, depth + 1))
                        return true;
                }
                else if (step.config.Uses(other))
                {
                    return true;
                }
            }

            return false;
        }

        private static int buildDepth;

        protected override Tween CreateTween(Component target)
        {
            if (buildDepth > 16)
            {
                Debug.LogWarning(
                    $"{name}: Sequence nesting too deep (cycle?).", this);

                return null;
            }

            buildDepth++;

            try
            {
                return CreateSequence(target);
            }
            finally
            {
                buildDepth--;
            }
        }

        private Sequence CreateSequence(Component target)
        {
            Sequence sequence = DOTween.Sequence();

            foreach (Step step in steps)
            {
                if (step == null || step.config == null)
                    continue;

                Tween child = step.config.BuildTween(target);

                if (child == null)
                    continue;

                if (step.mode == JoinMode.Join)
                {
                    sequence.Join(child);
                    continue;
                }

                if (step.interval > 0f)
                    sequence.AppendInterval(step.interval);

                sequence.Append(child);
            }

            return sequence;
        }
    }
}
