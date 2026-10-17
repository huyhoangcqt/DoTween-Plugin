using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using YellowCat.TweenPlugin;

public partial class UIInteraction
{
    [SerializeField] private List<UIAnimationPlayer> animationPlayers;

    private void PlayAlls()
    {
        if (animationPlayers.Count == 0)
        {
            return;
        }
        foreach (var animPlayer in animationPlayers)
        {
            animPlayer?.Play();
        }
    }

    private void RestartAlls()
    {
        if (animationPlayers.Count == 0)
        {
            return;
        }
        foreach (var animPlayer in animationPlayers)
        {
            animPlayer?.Restart();
        }
    }
}