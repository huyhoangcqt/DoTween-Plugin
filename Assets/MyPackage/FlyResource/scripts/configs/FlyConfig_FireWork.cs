using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin.Fly
{
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/FlyConfig/FireWork",
        fileName = "FlyConfig_FireWork")]
    public class FlyConfig_FireWork : FlyConfig
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
}
