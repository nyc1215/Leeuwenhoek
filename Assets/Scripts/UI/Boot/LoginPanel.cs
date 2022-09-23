using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BestHTTP.WebSocket;
using FairyGUI;
using Manager;
using MyWebSocket.Request;
using Newtonsoft.Json;
using UI.Util;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Boot
{
    public class LoginPanel
    {
        private readonly Window _window;
        private readonly GComponent _loginUIComponent;
        private readonly GButton _loginUIBackButton;
        private readonly GButton _loginButton;
        private readonly GTextInput _loginUITextInput;

        /// <summary>
        /// UI的初始化
        /// </summary>
        public LoginPanel(GComponent uiRoot)
        {
            _loginUIComponent = UIPackage.CreateObject("Boot", "Login").asCom;
            Assert.IsNotNull(_loginUIComponent);

            _loginUIBackButton = _loginUIComponent.GetChild("Button_Back").asButton;
            _loginButton = _loginUIComponent.GetChild("Button_Login").asButton;
            _loginUITextInput = _loginUIComponent.GetChild("Input_Account").asTextInput;

            Assert.IsNotNull(_loginUIBackButton);
            Assert.IsNotNull(_loginButton);
            Assert.IsNotNull(_loginUITextInput);

            _window = new Window
            {
                contentPane = _loginUIComponent,
                closeButton = _loginUIBackButton,
                pivot = Vector2.zero,
                pivotAsAnchor = true,
                modal = true
            };
            _loginButton.onClick.Add(Login);
            _loginUITextInput.onFocusIn.Add(() => { _loginUITextInput.promptText = string.Empty; });
            _loginUIBackButton.onClick.Add(() => { uiRoot.filter = null; });
        }

        public void Show()
        {
            _loginUITextInput.promptText = "请输入账号";
            _window.Show();
            _window.Center();
            MyWebSocket.MyWebSocket.Instance.Connect();
        }

        private void Login()
        {
            MyWebSocket.MyWebSocket.Instance.Connect();

            if (_loginUITextInput.text.Equals(string.Empty))
            {
                BootUIPanel.TipPanel.Show("账号为空，请输入账号");
                return;
            }

            if (MyWebSocket.MyWebSocket.WebSocket.State != WebSocketStates.Open)
            {
                BootUIPanel.TipPanel.ShowNetWorkError();
                return;
            }

            MyGameManager.Instance.LocalPlayerInfo.Account = _loginUITextInput.text;
            var requestLogin = new RequestLogin(_loginUITextInput.text);
            requestLogin.RequestSuccess += () =>
            {
                _window.Hide();
                if (GRoot.inst.GetChild("ChoosePanel") == null)
                {
                    GRoot.inst.AddChild(BootUIPanel.ChoosePanelComponent);
                }

                BootUIPanel.ChoosePanelComponent.visible = true;
                BootUIPanel.ChoosePanelComponent.Center();
            };
            requestLogin.RequestFail += () => { BootUIPanel.TipPanel.Show("该用户账号已登录或不存在，请先注册"); };

            MyGameManager.Instance.NetWorkOperations.SendRequest(requestLogin);
        }
    }
}