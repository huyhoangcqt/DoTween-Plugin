using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using YellowCat.TweenPlugin.Fly;
using UnityEngine.UI;

public partial class Scene_Demo_FlyResourceMgr : MonoBehaviour
{
    [SerializeField] private CurrencyController currencyController;
    [SerializeField] private FlyStyle flyStyle;
    
    [Header("Scene")]
    [SerializeField] private Button btnFlyGold;
    [SerializeField] private Button btnFlyMana;
    
    [Header("Source Object")]
    [SerializeField] private Transform objGold;
    [SerializeField] private Transform objMana;

    [Header("Example value")]
    [SerializeField] private int currentAmount = 100;
    [SerializeField] private int rewardAmount = 28;

    private void Awake()
    {
        btnFlyGold.onClick.AddListener(() => FlyResource(CurrencyType.Gold));
        btnFlyMana.onClick.AddListener(() => FlyResource(CurrencyType.Mana));
    }

    private void FlyResource(CurrencyType type)
    {
        CurrencyResource resource = GetCurrencyResource(type);
        var sourceObject = GetSourceObject(type);
        FlyResourceMgr.Instance.FlyResource(flyStyle, sourceObject, resource.CurrencyBar, resource.CoinPrefab, currentAmount, rewardAmount, null, OnFlyComplete);
    }

    public CurrencyResource GetCurrencyResource(CurrencyType type)
    {
        return currencyController.GetCurrencyResource(type);
    }

    public Transform GetSourceObject(CurrencyType type)
    {
        switch (type)
        {
            case CurrencyType.Gold:
                return objGold;
            case CurrencyType.Mana:
                return objMana;
            default:
                return null;
        }
    }

    private void OnFlyComplete()
    {
        Debug.Log("Fly complete!");
    }

}