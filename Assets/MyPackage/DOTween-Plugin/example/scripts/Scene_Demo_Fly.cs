using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YellowCat.TweenPlugin.Fly;

/// <summary>
/// Demo: press the button and a burst of coins flies from the source
/// to the currency UI. Only "source", "currencyIcon" and "buttonFly" are required;
/// the coin template and FlyConfig are created at runtime when left empty.
/// </summary>
public partial class Scene_Demo_Fly : MonoBehaviour
{
    [Header("Scene")]
    [SerializeField] private Button buttonFly;
    [SerializeField] private RectTransform source;
    [Tooltip("FlyTarget on the CurrencyBar: defines icon and text.")]
    [SerializeField] private FlyTarget flyTarget;

    [Header("Fly")]
    [SerializeField] private FlyConfig flyConfig;
    [Tooltip("Optional. If empty, a simple yellow square is generated.")]
    [SerializeField] private RectTransform coinTemplate;

    [Header("Reward")]
    [SerializeField, Min(0)] private int currentValue = 100;
    [SerializeField, Min(0)] private int rewardAmount = 28;

    private readonly List<RectTransform> pool = new List<RectTransform>();

    private void Awake()
    {
        if (flyConfig == null)
        {
            flyConfig = ScriptableObject.CreateInstance<FlyConfigCurved>();
            flyConfig.hideFlags = HideFlags.DontSave;
        }

        if (coinTemplate != null)
            coinTemplate.gameObject.SetActive(false);

        if (buttonFly != null)
            buttonFly.onClick.AddListener(Fly);

        if (flyTarget != null)
            flyTarget.SetValue(currentValue);
    }

    private void OnDestroy()
    {
        if (buttonFly != null)
            buttonFly.onClick.RemoveListener(Fly);
    }

    public void Fly()
    {
        if (source == null || flyTarget == null)
        {
            Debug.LogWarning("Scene_Demo_Fly: assign source and flyTarget.", this);
            return;
        }

        int before = currentValue;
        int after = before + rewardAmount;

        // Commit the new value right away; the UI counts up while coins land.
        currentValue = after;

        FlyEffect.PlayCurrency(
            flyConfig,
            before,
            after,
            source,
            flyTarget,
            () =>
            {
                RectTransform coin = GetCoin();
                coin.gameObject.SetActive(true);
                return coin;
            },
            style: flyConfig.Style);
    }

    // deactivateOnArrive turns finished coins off, so any inactive entry is free.
    private RectTransform GetCoin()
    {
        foreach (RectTransform coin in pool)
        {
            if (!coin.gameObject.activeSelf)
                return coin;
        }

        RectTransform created = coinTemplate != null
            ? Instantiate(coinTemplate)
            : CreateDefaultCoin();

        created.SetParent(flyTarget.Icon.root, false);
        created.SetAsLastSibling();
        pool.Add(created);
        return created;
    }

    private RectTransform CreateDefaultCoin()
    {
        var go = new GameObject("Coin", typeof(RectTransform), typeof(Image));
        var rect = (RectTransform)go.transform;
        rect.sizeDelta = new Vector2(40f, 40f);

        var image = go.GetComponent<Image>();
        image.color = new Color(1f, 0.82f, 0.1f);
        image.raycastTarget = false;
        return rect;
    }
}