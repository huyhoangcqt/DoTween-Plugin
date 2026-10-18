using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using YellowCat.TweenPlugin.Fly;

public partial class CurrencyController : MonoBehaviour
{
    [SerializeField] private List<CurrencyResource> currencyResources;

    public CurrencyResource GetCurrencyResource(CurrencyType type)
    {
        return currencyResources.Find(resource => resource.Type == type);
    }
}

public enum CurrencyType
{
    Gold,
    Mana,
}

[System.Serializable]
public struct CurrencyResource
{
    public CurrencyType Type;
    public FlyTarget CurrencyBar;
    public GameObject CoinPrefab;
}