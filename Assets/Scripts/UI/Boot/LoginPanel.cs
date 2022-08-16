using System.Collections;
using System.Collections.Generic;
using BestHTTP.WebSocket;
using FairyGUI;
using MyWebSocket.Request;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Boot
{
    public static class LoginPanel
    {
        private static GComponent _loginUIComponent;
        private static GButton _loginUIBackButton;
        private static GButton _loginButton;
        private static GTextInput _loginUITextInput;

        /// <summary>
        /// 提示UI的初始化
        /// </summary>
        public static void LoginPanelInit(GComponent uiRoot)
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

            _loginUIComponent.visible = false;
            _loginUIBackButton.onClick.Add((() => { _loginUIComponent.visible = false; }));
            _loginButton.onClick.Add(Login);

            GRoot.inst.AddChild(_loginUIComponent);
        }

        public static void Show()
        {
            _loginUIComponent.visible = true;
            MyWebSocket.MyWebSocket.Instance.Connect();
        }

        private static void Login()
        {
            if (_loginUITextInput.text.Equals(string.Empty))
            {
                TipPanel.Show("请输入昵称");
                return;
            }

            if (MyWebSocket.MyWebSocket.Instance.WebSocket.State != WebSocketStates.Open)
            {
                TipPanel.ShowNetWorkError();
                return;
            }

            MyWebSocket.MyWebSocket.Instance.Send(new RequestLogin(_loginUITextInput.text).ToJson());
        }
    }
}