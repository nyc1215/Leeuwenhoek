using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace MyUI
{
    /// <summary>
    /// 启动界面UI
    /// </summary>
    public class BootUIPannel : MonoBehaviour
    {
        public string startBtnToScene;

        private UIPanel _Panel;
        private GComponent _UIroot;

        private GButton _QuitButton;
        private GButton _StartButton;

        private void Awake()
        {
            _Panel = GetComponent<UIPanel>();
            _UIroot = _Panel.ui;

            Assert.IsNotNull(_Panel);
            Assert.IsNotNull(_UIroot);
        }

        private void Start()
        {
            _QuitButton = _UIroot?.GetChild("Button_Quit").asButton;
            _StartButton = _UIroot?.GetChild("Button_Start").asButton;

            _QuitButton?.onClick.Add(() => { UIOperationUtil.QuitGame(); });
            _StartButton?.onClick.Add(() =>
            {
                UIOperationUtil.GoToScene(startBtnToScene);
            });
        }
    }
}
