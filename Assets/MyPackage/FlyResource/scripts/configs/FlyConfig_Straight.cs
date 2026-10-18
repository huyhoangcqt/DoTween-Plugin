using DG.Tweening;
using UnityEngine;

namespace YellowCat.TweenPlugin.Fly
{
    // Defaults are applied in Reset(), i.e. only when the asset is first created.
    [CreateAssetMenu(
        menuName = "YellowCat/Tween/FlyConfig/Straight",
        fileName = "FlyConfig_Straight")]
    public class FlyConfig_Straight : FlyConfig
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
}
