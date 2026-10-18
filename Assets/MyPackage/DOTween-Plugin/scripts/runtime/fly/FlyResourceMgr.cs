//Một lớp quản lý tài nguyên cho hiệu ứng bay (Fly Effect) trong Unity.
//Chứa utilities và quản lý các tài nguyên cần thiết cho hiệu ứng bay.

using YellowCat.TweenPlugin.Fly;
using System.Collections.Generic;
using UnityEngine;
using System;

public partial class FlyResourceMgr : MonoBehaviour
{

    #region Singleton:
    private static FlyResourceMgr instance;

    public static FlyResourceMgr Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<FlyResourceMgr>();
                if (instance == null)
                {
                    GameObject go = new GameObject("FlyResourceMgr");
                    instance = go.AddComponent<FlyResourceMgr>();
                }
            return instance;
        }
    }
    #endregion!!!
}

[Serializable]
public class FlyResource
{
    public FlyStyle flyStyle;
    public FlyConfig flyConfig;
}