using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin.Fly
{
    // Defaults are applied in Reset(), i.e. only when the asset is first created.

    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Fly/FlyConfig/Straight",
        fileName = "FlyConfig_Straight")]
    public class FlyConfigStraight : FlyConfig
    {
        public override FlyStyle Style => FlyStyle.FlyStraight;

        private void Reset()
        {
            duration = 0.5f;
            ease = Ease.InQuad;
            arcHeight = 0f;
            spread = 0f;
            gapInterval = 0.05f;
        }
    }

    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Fly/FlyConfig/Curved",
        fileName = "FlyConfig_Curved")]
    public class FlyConfigCurved : FlyConfig
    {
        public override FlyStyle Style => FlyStyle.FlyCurved;

        private void Reset()
        {
            duration = 0.6f;
            ease = Ease.InQuad;
            arcHeight = 100f;
            spread = 80f;
            gapInterval = 0.05f;
        }
    }

    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Fly/FlyConfig/FireWork",
        fileName = "FlyConfig_FireWork")]
    public class FlyConfigFireWork : FlyConfig
    {
        public override FlyStyle Style => FlyStyle.FlyFirework;

        private void Reset()
        {
            duration = 0.5f;
            ease = Ease.InCubic;
            burstRadius = 180f;
            burstDuration = 0.3f;
            gapInterval = 0.03f;
        }
    }

    [CreateAssetMenu(
        menuName = "YellowCat/Tween/Fly/FlyConfig/Blooming",
        fileName = "FlyConfig_Blooming")]
    public class FlyConfigBlooming : FlyConfig
    {
        public override FlyStyle Style => FlyStyle.FlyBlooming;

        private void Reset()
        {
            duration = 0.5f;
            ease = Ease.InBack;
            burstRadius = 150f;
            burstDuration = 0.35f;
            holdDuration = 0.25f;
            gapInterval = 0.04f;
        }
    }
}
