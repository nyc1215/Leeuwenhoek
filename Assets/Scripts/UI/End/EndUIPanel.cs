using System;
using FairyGUI;
using Manager;
using UI.Util;
using UnityEngine;

namespace UI.End
{
    public class EndUIPanel : UIPanelUtil
    {
        private GButton _quitButton;

        protected override void Awake()
        {
            base.Awake();

            _quitButton = GetButton("Button_Back");
        }

        private void Start()
        {
            _quitButton.onClick.Add(() => { UIOperationUtil.GoToScene(MyGameManager.Instance.uiJumpData.bootMenu); });
        }
    }
}