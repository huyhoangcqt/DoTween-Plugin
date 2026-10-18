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

        Canvas referenceCanvas = ResolveRootCanvas(target.Icon);
        int finalValue = currentValue + rewardValue;
        var run = new Run { target = target, finalValue = finalValue };

        run.sequence = FlyEffect.PlayCurrency(
            config,
            currentValue,
            finalValue,
            source,
            target,
            () => GetCoin(coinPrefab, referenceCanvas),
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

        foreach (CoinPool pool in pools.Values)
        {
            foreach (Transform coin in pool.coins)
            {
                if (coin != null)
                    coin.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>Creates coins of a prefab in advance to avoid spikes on the first reward.</summary>
    /// <param name="referenceCanvas">Canvas to match if FlyCanvas has to be created (optional).</param>
    public void Prewarm(GameObject coinPrefab, int count, Canvas referenceCanvas = null)
    {
        CoinPool pool = GetPool(coinPrefab, referenceCanvas);

        for (int i = 0; i < count; i++)
            CreateCoin(coinPrefab, pool);
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
    private class CoinPool
    {
        public Transform container;
        public readonly List<Transform> coins = new List<Transform>();
    }

    private readonly Dictionary<int, CoinPool> pools =
        new Dictionary<int, CoinPool>();

    // Layout: FlyCanvas / poolParent / <prefab name> / <prefab name>(Clone) ...
    // Coins never leave their container; they are only toggled with SetActive.
    private CoinPool GetPool(GameObject coinPrefab, Canvas reference = null)
    {
        int key = coinPrefab != null ? coinPrefab.GetInstanceID() : 0;

        if (pools.TryGetValue(key, out CoinPool pool) && pool.container != null)
            return pool;

        EnsurePoolParent(reference);

        pool = pool ?? new CoinPool();
        var container = new GameObject(
            coinPrefab != null ? coinPrefab.name : "Coin",
            typeof(RectTransform));

        container.transform.SetParent(poolParent, false);
        pool.container = container.transform;
        pools[key] = pool;
        return pool;
    }

    // Uses the assigned poolParent if set. Otherwise creates a dedicated
    // FlyCanvas, so moving coins only rebuild this small canvas and never
    // the game's main UI canvases.
    private void EnsurePoolParent(Canvas reference)
    {
        if (poolParent != null)
            return;

        var canvasGo = new GameObject("FlyCanvas", typeof(RectTransform));
        canvasGo.transform.SetParent(transform, false);

        var canvas = canvasGo.AddComponent<Canvas>();
        canvas.overrideSorting = true;
        canvas.sortingOrder = flyCanvasSortingOrder;

        // Match the reference canvas so world positions line up.
        if (reference != null &&
            reference.renderMode != RenderMode.WorldSpace)
        {
            canvas.renderMode = reference.renderMode;
            canvas.worldCamera = reference.worldCamera;
            canvas.planeDistance = reference.planeDistance;
            canvas.sortingLayerID = reference.sortingLayerID;
        }
        else
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        var poolGo = new GameObject("PoolParent", typeof(RectTransform));
        poolGo.transform.SetParent(canvasGo.transform, false);
        poolParent = poolGo.transform;
    }

    private static Canvas ResolveRootCanvas(Transform from)
    {
        Canvas canvas = from.GetComponentInParent<Canvas>();
        return canvas != null ? canvas.rootCanvas : null;
    }

    // Finished coins are inactive, so any inactive entry is free.
    private Transform GetCoin(GameObject coinPrefab, Canvas reference)
    {
        CoinPool pool = GetPool(coinPrefab, reference);
        pool.coins.RemoveAll(c => c == null);

        Transform coin = null;

        foreach (Transform candidate in pool.coins)
        {
            if (!candidate.gameObject.activeSelf)
            {
                coin = candidate;
                break;
            }
        }

        if (coin == null)
            coin = CreateCoin(coinPrefab, pool);

        coin.gameObject.SetActive(true);
        return coin;
    }

    private Transform CreateCoin(GameObject coinPrefab, CoinPool pool)
    {
        Transform coin;

        if (coinPrefab != null)
        {
            coin = Instantiate(coinPrefab, pool.container, false).transform;
        }
        else
        {
            var go = new GameObject(
                "Coin(Clone)", typeof(RectTransform), typeof(Image));

            go.transform.SetParent(pool.container, false);
            ((RectTransform)go.transform).sizeDelta = new Vector2(40f, 40f);

            var image = go.GetComponent<Image>();
            image.color = new Color(1f, 0.82f, 0.1f);
            image.raycastTarget = false;
            coin = go.transform;
        }

        coin.gameObject.SetActive(false);
        pool.coins.Add(coin);
        return coin;
    }

    #endregion Handle Internal!!!
}
