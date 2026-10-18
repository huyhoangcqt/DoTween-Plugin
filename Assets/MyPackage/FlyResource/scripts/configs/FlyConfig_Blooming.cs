using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin.Fly
{
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/FlyConfig/Blooming",
        fileName = "FlyConfig_Blooming")]
    public class FlyConfig_Blooming : FlyConfig
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
