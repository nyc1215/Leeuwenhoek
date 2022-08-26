using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Manager;
using MyWebSocket.Request;
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
        private readonly Transition _matchingTextAnimate;

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
            _matchingTextAnimate = _matchingUIComponent.GetTransition("匹配文字动画");

            Assert.IsNotNull(_matchingUIBackButton);
            Assert.IsNotNull(_matchingUIText);
            Assert.IsNotNull(_matchingTextAnimate);

            _window = new Window
            {
                contentPane = _matchingUIComponent,
                closeButton = _matchingUIBackButton,
                modal = true
            };

            _matchingUIBackButton.onClick.Add(CancelMatching);
        }

        public void Show()
        {
            var requestMatching = new RequestMatching(MyGameManager.Instance.LocalPlayerInfo.Account, MyGameManager.Instance.LocalPlayerInfo.ScriptName);
            requestMatching.RequestSuccess += () =>
            {
                BootUIPanel.ChoosePanelComponent.visible = false;
                BootUIPanel.InfoPanelComponent.visible = false;
            };
            MyGameManager.Instance.NetWorkOperations.SendRequest(requestMatching);

            _window.Show();
            _matchingTextAnimate.Play(-1, 0, null);
            _window.BringToFront();
        }

        private void CancelMatching()
        {
            var requestCancelMatching = new RequestCancelMatching(MyGameManager.Instance.LocalPlayerInfo.Account, MyGameManager.Instance.LocalPlayerInfo.ScriptName);
            MyGameManager.Instance.NetWorkOperations.SendRequest(requestCancelMatching);
            _matchingTextAnimate.Stop();
            _window.Hide();
        }
    }
}