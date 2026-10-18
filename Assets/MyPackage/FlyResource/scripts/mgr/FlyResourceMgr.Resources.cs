using YellowCat.TweenPlugin.Fly;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace YellowCat.TweenPlugin.Fly
{
    //FlyResourceMgr.Resources
    public partial class FlyResourceMgr
    {
        #region Resources:

        // Add resource management methods and properties here.
        [SerializeField] private FlyStyle defaultStyle = FlyStyle.FlyCurved;
        [SerializeField] private List<FlyResource> flyConfigs;
        private Transform poolParent = null;

        [Tooltip("Sorting order of the auto-created FlyCanvas (used when poolParent is empty).")]
        [SerializeField] private int flyCanvasSortingOrder = 100;

        #endregion

        private FlyConfig GetConfig(FlyStyle flyStyle)
        {
            foreach (var resource in flyConfigs)
            {
                if (resource.flyStyle == flyStyle)
                    return resource.flyConfig;
            }
            return null;
        }
    }
}