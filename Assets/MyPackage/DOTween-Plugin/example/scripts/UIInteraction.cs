using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using YellowCat.TweenPlugin;

public partial class UIInteraction : MonoBehaviour
{
    [SerializeField] private Button buttonPlay;

    private void Awake()
    {
        buttonPlay.onClick.AddListener(() => {
            RestartAlls();
        });
    }

    private void Start()
    {
        PlayAlls();
    }
}