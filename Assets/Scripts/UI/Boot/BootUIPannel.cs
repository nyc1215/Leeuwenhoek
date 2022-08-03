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
    public class BootUIPannel : UIPannelUtil
    {
        [Scene]public string startBtnToScene;

        private UIPanel _panel;
        private GComponent _uiRoot;

        private GButton _quitButton;
        private GButton _startButton;

        private void Awake()
        {
            _panel = GetComponent<UIPanel>();
            _uiRoot = _panel.ui;

            Assert.IsNotNull(_panel);
            Assert.IsNotNull(_uiRoot);
        }

        private void Start()
        {
            _quitButton = _uiRoot?.GetChild("Button_Quit").asButton;
            _startButton = _uiRoot?.GetChild("Button_Start").asButton;

            _quitButton?.onClick.Add(UIOperationUtil.QuitGame);
            _startButton?.onClick.Add(() => { UIOperationUtil.GoToScene(startBtnToScene); });
        }
    }
}