using FairyGUI;
using Mirror;
using UI.Util;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Boot
{
    /// <summary>
    /// 启动界面UI
    /// </summary>
    public class BootUIPanel : UIPanelUtil
    {
        private GButton _quitButton;
        private GButton _startButton;

        private void Start()
        {
            _quitButton = GetButton("Button_Quit");
            _startButton = GetButton("Button_Start");

            _quitButton?.onClick.Add(UIOperationUtil.QuitGame);
            _startButton?.onClick.Add(() => { UIOperationUtil.GoToScene(UIJumpData.instance.startMenu); });
        }
    }
}