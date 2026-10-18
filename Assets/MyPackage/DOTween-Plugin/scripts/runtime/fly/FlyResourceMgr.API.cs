using DG.Tweening;
using YellowCat.TweenPlugin.Fly;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public partial class FlyResourceMgr
{
    #region Fly Utils Methods:

    /// <summary>
    /// Flies coins from source to target to give a currency reward.
    /// The number on the target counts up from currentValue to
    /// currentValue + rewardValue as coins arrive.
    /// </summary>
    /// <param name="flyStyle">Look of the flight; its FlyConfig comes from the Resources list.</param>
    /// <param name="source">Where coins start (position and rotation).</param>
    /// <param name="target">CurrencyBar's FlyTarget (icon + text).</param>
    /// <param name="coinPrefab">Coin to spawn. Null falls back to a simple yellow square.</param>
    /// <param name="currentValue">Value shown before the reward.</param>
    /// <param name="rewardValue">Amount added by the reward.</param>
    /// <param name="onValueChanged">Called with the displayed value after each coin lands.</param>
    /// <param name="onComplete">Called once, after the last coin lands.</param>
    /// <returns>The running sequence, or null if nothing flies.</returns>
    public Sequence FlyResource(FlyStyle flyStyle, Transform source, FlyTarget target, GameObject coinPrefab,
        int currentValue, int rewardValue,
        Action<int> onValueChanged = null, Action onComplete = null)
    {
        if (source == null || target == null)
        {
            Debug.LogWarning("FlyResourceMgr: source and target are required.", this);
            return null;
        }

        FlyConfig config = GetConfig(flyStyle);

        if (config == null)
        {
            Debug.LogWarning(
                $"FlyResourceMgr: no FlyConfig registered for {flyStyle}.", this);
            return null;
        }

        Transform canvasRoot = ResolveCanvasRoot(target.Icon);
        int finalValue = currentValue + rewardValue;
        var run = new Run { target = target, finalValue = finalValue };

        run.sequence = FlyEffect.PlayCurrency(
            config,
            currentValue,
            finalValue,
            source,
            target,
            () => GetCoin(coinPrefab, canvasRoot),
            onValueChanged,
            () =>
            {
                activeRuns.Remove(run);
                onComplete?.Invoke();
            },
            flyStyle);

        if (run.sequence != null)
            activeRuns.Add(run);

        return run.sequence;
    }

    /// <summary>Same as above, using the default style set on the manager.</summary>
    public Sequence FlyResource(
        Transform source,
        FlyTarget target,
        GameObject coinPrefab,
        int currentValue,
        int rewardValue,
        Action<int> onValueChanged = null,
        Action onComplete = null)
    {
        return FlyResource(
            defaultStyle, source, target, coinPrefab, currentValue, rewardValue,
            onValueChanged, onComplete);
    }

    /// <summary>True while any reward is still flying.</summary>
    public bool IsFlying => activeRuns.Count > 0;

    /// <summary>
    /// Stops every running fly effect and hides the coins.
    /// With snapToFinal, each target shows its final value immediately.
    /// </summary>
    public void StopAll(bool snapToFinal = true)
    {
        foreach (Run run in activeRuns.ToArray())
        {
            run.sequence?.Kill();

            if (snapToFinal && run.target != null)
                run.target.SetValue(run.finalValue);
        }

        activeRuns.Clear();

        foreach (List<Transform> list in pools.Values)
        {
            foreach (Transform coin in list)
            {
                if (coin != null)
                    coin.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>Creates coins of a prefab in advance to avoid spikes on the first reward.</summary>
    public void Prewarm(GameObject coinPrefab, int count, Transform parent)
    {
        for (int i = 0; i < count; i++)
        {
            Transform coin = CreateCoin(coinPrefab, parent);
            coin.gameObject.SetActive(false);
        }
    }

    #endregion Fly Utils Methods!!!

    #region Handle Internal:

    private class Run
    {
        public Sequence sequence;
        public FlyTarget target;
        public int finalValue;
    }

    private readonly List<Run> activeRuns = new List<Run>();

    // One pool per coin prefab (key 0 = the built-in fallback coin).
    private readonly Dictionary<int, List<Transform>> pools =
        new Dictionary<int, List<Transform>>();

    private List<Transform> GetPool(GameObject coinPrefab)
    {
        int key = coinPrefab != null ? coinPrefab.GetInstanceID() : 0;

        if (!pools.TryGetValue(key, out List<Transform> list))
        {
            list = new List<Transform>();
            pools[key] = list;
        }

        return list;
    }

    // Coins must live under a Canvas to be drawn; use the target's root canvas.
    private static Transform ResolveCanvasRoot(Transform from)
    {
        Canvas canvas = from.GetComponentInParent<Canvas>();
        return canvas != null ? canvas.rootCanvas.transform : from.root;
    }

    // deactivateOnArrive hides finished coins, so any inactive entry is free.
    private Transform GetCoin(GameObject coinPrefab, Transform canvasRoot)
    {
        List<Transform> pool = GetPool(coinPrefab);
        pool.RemoveAll(c => c == null);

        foreach (Transform coin in pool)
        {
            if (coin.gameObject.activeSelf)
                continue;

            if (coin.parent != canvasRoot)
                coin.SetParent(canvasRoot, false);

            coin.SetAsLastSibling();
            coin.gameObject.SetActive(true);
            return coin;
        }

        Transform created = CreateCoin(coinPrefab, canvasRoot);
        created.gameObject.SetActive(true);
        return created;
    }

    private Transform CreateCoin(GameObject coinPrefab, Transform parent)
    {
        Transform coin;

        if (coinPrefab != null)
        {
            coin = Instantiate(coinPrefab, parent, false).transform;
        }
        else
        {
            var go = new GameObject(
                "Coin", typeof(RectTransform), typeof(Image));

            go.transform.SetParent(parent, false);
            ((RectTransform)go.transform).sizeDelta = new Vector2(40f, 40f);

            var image = go.GetComponent<Image>();
            image.color = new Color(1f, 0.82f, 0.1f);
            image.raycastTarget = false;
            coin = go.transform;
        }

        coin.SetAsLastSibling();
        GetPool(coinPrefab).Add(coin);
        return coin;
    }

    #endregion Handle Internal!!!
}
