using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin.Fly
{
    /// <summary>
    /// Settings of a "fly from A to B" effect (coins, gems, stars...).
    /// Scene objects (origin, destination) are passed at runtime via FlyContext.
    /// </summary>
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Fly/Fly Config",
        fileName = "FlyConfig")]
    public class FlyConfig : ScriptableObject
    {
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
