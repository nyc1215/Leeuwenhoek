using FairyGUI;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Game
{
    public class GameTipPanel
    {
        private readonly Window _window;
        private readonly GComponent _tipUIComponent;
        private readonly GButton _tipUIBackButton;
        private readonly GTextField _tipUIText;

        /// <summary>
        /// 提示UI的初始化
        /// </summary>
        public GameTipPanel()
        {
            _tipUIComponent = UIPackage.CreateObject("Game", "Tip").asCom;
            Assert.IsNotNull(_tipUIComponent);

            _tipUIBackButton = _tipUIComponent.GetChild("Button_Back").asButton;
            _tipUIText = _tipUIComponent.GetChild("Title").asTextField;

            Assert.IsNotNull(_tipUIBackButton);
            Assert.IsNotNull(_tipUIText);


            _window = new Window
            {
                contentPane = _tipUIComponent,
                closeButton = _tipUIBackButton,
                pivot = Vector2.zero,
                pivotAsAnchor = true,
                modal = true
            };
        }

        public void Show(string tips)
        {
            if (string.IsNullOrEmpty(tips))
            {
                return;
            }
            
            _tipUIText.text = tips;
            _window.Show();
            _window.Center();
            _window.BringToFront();
        }
    }
}