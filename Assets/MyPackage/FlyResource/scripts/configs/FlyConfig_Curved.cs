using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin.Fly
{
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/FlyConfig/Curved",
        fileName = "FlyConfig_Curved")]
    public class FlyConfig_Curved : FlyConfig
    {
        public override FlyStyle Style => FlyStyle.FlyCurved;

        private void Reset()
        {
            duration = 0.6f;
            ease = Ease.InQuad;
            arcHeight = 250f;
            spread = 80f;
            gapInterval = 0.1f;
        }
    }
}
