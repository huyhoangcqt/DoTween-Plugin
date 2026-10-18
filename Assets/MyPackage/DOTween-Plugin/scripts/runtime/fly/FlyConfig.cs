using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin.Fly
{
    /// <summary>
    /// Base settings of a "fly from A to B" effect (coins, gems, stars...).
    /// Each FlyStyle has its own subclass (see FlyConfigs.cs) with its own defaults.
    /// Scene objects (origin, destination) are passed at runtime via FlyContext.
    /// </summary>
    public abstract class FlyConfig : ScriptableObject
    {
        public abstract FlyStyle Style { get; }

        [Header("Timing")]
        [Min(0.01f)] public float duration = 0.6f;
        [Min(0f)] public float delay;
        public Ease ease = Ease.InQuad;
        public bool ignoreTimeScale;

        [Header("Path")]
        [Tooltip("How far the path bows away from the straight line (world units).")]
        public float arcHeight = 100f;

        [Tooltip("Random offset of the path's control point, per item (world units).")]
        [Min(0f)] public float spread = 80f;

        [Header("Burst")]
        [Tooltip("Maximum number of coins flying out for one reward.")]
        [Min(1)] public int maxCoin = 10;

        [Tooltip("Spawn gap (seconds) between two consecutive coins.")]
        [UnityEngine.Serialization.FormerlySerializedAs("stagger")]
        [Min(0f)] public float gapInterval = 0.05f;

        [Header("Style: Firework / Blooming")]
        [Tooltip("How far items burst out before flying to the target (world units).")]
        [Min(0f)] public float burstRadius = 150f;

        [Min(0.01f)] public float burstDuration = 0.3f;

        [Tooltip("Blooming only: pause after the burst, before flying.")]
        [Min(0f)] public float holdDuration = 0.25f;

        [Header("Transform")]
        [Tooltip("Start exactly at the origin's position and rotation.")]
        public bool snapToOrigin = true;

        public bool matchDestinationRotation;

        [Tooltip("Scale multiplier reached on arrival (relative to the start scale).")]
        public float endScaleMultiplier = 1f;

        [Header("On Arrive")]
        public bool deactivateOnArrive = true;
    }
}
