using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using YellowCat.TweenPlugin;

public class PopupBase : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] private Transform panel;
    [SerializeField] private Button buttonClose;
    [SerializeField] private UIAnimationPlayer animCtrl;
    
    [Header("Configs")]
    [SerializeField] private TweenConfigBase tweenConfig_Intro;
    [SerializeField] private TweenConfigBase tweenConfig_Outro;

    #region Main:

    // Start is called before the first frame update
    void Start()
    {
        buttonClose.onClick.AddListener(() => {
            Close();
        });
    }

    public void Open()
    {
        _playAnimIntro();
    }

    public void Close()
    {
        _playAnimOutro();
    }
    
    #endregion Main!!!

    #region Private Method:
    public void _playAnimIntro()
    {
        animCtrl.Stop();
        animCtrl.Play(tweenConfig_Intro);
    }
    
    public void _playAnimOutro()
    {
        animCtrl.Stop();
        animCtrl.Play(tweenConfig_Outro);
    }
    #endregion Private Method!!!
}
