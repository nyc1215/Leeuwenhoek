using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BestHTTP.WebSocket;
using FairyGUI;
using MyWebSocket.Request;
using Newtonsoft.Json;
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
        /// 提示UI的初始化
        /// </summary>
        public LoginPanel([NotNull] TipPanel tipPanel)
        {
            _loginUIComponent = UIPackage.CreateObject("Boot", "Login").asCom;
            _loginUIComponent.Center();
            Assert.IsNotNull(_loginUIComponent);

            _loginUIBackButton = _loginUIComponent.GetChild("Button_Back").asButton;
            _loginButton = _loginUIComponent.GetChild("Button_Login").asButton;
            _loginUITextInput = _loginUIComponent.GetChild("name").asTextInput;

            Assert.IsNotNull(_loginUIBackButton);
            Assert.IsNotNull(_loginButton);
            Assert.IsNotNull(_loginUITextInput);
            
            _window = new Window()
            {
                contentPane = _loginUIComponent,
                closeButton = _loginUIBackButton,
                modal = true
            };
            _loginButton.onClick.Add((() => { Login(tipPanel); }));
        }

        public void Show()
        {
            _window.Show();
            MyWebSocket.MyWebSocket.Instance.Connect();
        }

        private void Login(TipPanel tipPanel)
        {
            if (_loginUITextInput.text.Equals(string.Empty))
            {
                tipPanel.Show("请输入昵称");
                return;
            }

            if (MyWebSocket.MyWebSocket.Instance.WebSocket.State != WebSocketStates.Open)
            {
                tipPanel.ShowNetWorkError();
                return;
            }

            MyWebSocket.MyWebSocket.Instance.Send(new RequestLogin(_loginUITextInput.text).ToJson());
        }
    }
}