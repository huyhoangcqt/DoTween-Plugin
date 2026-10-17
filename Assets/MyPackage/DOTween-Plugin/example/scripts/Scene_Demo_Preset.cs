using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using YellowCat.TweenPlugin;

public partial class Scene_Demo_Preset : MonoBehaviour
{
    [SerializeField] private Button buttonOpenPopup;
    [SerializeField] private PopupBase popupCtrl;

    private void Awake()
    {
        buttonOpenPopup.onClick.AddListener(() => {
            popupCtrl.Open();
        });
    }
}
