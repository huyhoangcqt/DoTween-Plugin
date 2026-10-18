using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace YellowCat.TweenPlugin.Fly
{
    /// <summary>
    /// Defines where coins fly to: the icon and the value text.
    /// Add it directly to the CurrencyBar.
    /// </summary>
    public class FlyTarget : MonoBehaviour
    {
        [SerializeField] private RectTransform icon;
        [SerializeField] private Text text;

        [SerializeField, Min(0f)] private float punchScale = 0.2f;
        [SerializeField, Min(0f)] private float punchDuration = 0.15f;

        /// <summary>Where coins fly to. Falls back to this object.</summary>
        public Transform Icon => icon != null ? icon : transform;

        public Text Text => text;

        public void SetValue(int value)
        {
            if (text != null)
                text.text = value.ToString();
        }

        public void Punch()
        {
            Transform t = Icon;
            DOTween.Kill(t, true);
            t.localScale = Vector3.one;
            t.DOPunchScale(Vector3.one * punchScale, punchDuration, 1, 0f);
        }
    }
}
