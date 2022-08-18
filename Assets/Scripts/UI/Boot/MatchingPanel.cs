using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Boot
{
    public class MatchingPanel
    {
        private readonly Window _window;
        private readonly GComponent _matchingUIComponent;
        private readonly GButton _matchingUIBackButton;
        private readonly GTextField _matchingUIText;

        /// <summary>
        /// UI的初始化
        /// </summary>
        public MatchingPanel()
        {
            _matchingUIComponent = UIPackage.CreateObject("Boot", "Matching").asCom;
            _matchingUIComponent.Center();
            Assert.IsNotNull(_matchingUIComponent);

            _matchingUIBackButton = _matchingUIComponent.GetChild("Button_Back").asButton;
            _matchingUIText = _matchingUIComponent.GetChild("Title").asTextField;

            Assert.IsNotNull(_matchingUIBackButton);
            Assert.IsNotNull(_matchingUIText);


            _window = new Window
            {
                contentPane = _matchingUIComponent,
                closeButton = _matchingUIBackButton,
                modal = true
            };
        }

        public void Show()
        {
            _window.Show();
            var matchingTextAnimate = _matchingUIComponent.GetTransition("匹配文字动画");
            matchingTextAnimate.Play(-1, 0, null);
            _window.BringToFront();
        }
    }
}