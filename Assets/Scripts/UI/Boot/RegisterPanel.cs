using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BestHTTP.WebSocket;
using FairyGUI;
using Manager;
using MyWebSocket.Request;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Boot
{
    public class RegisterPanel
    {
        private readonly Window _window;
        private readonly GComponent _registerUIComponent;
        private readonly GButton _registerUIBackButton;
        private readonly GButton _registerButton;
        private readonly GTextInput _registerUITextInput;

        /// <summary>
        /// 提示UI的初始化
        /// </summary>
        public RegisterPanel([NotNull] TipPanel tipPanel)
        {
            _registerUIComponent = UIPackage.CreateObject("Boot", "Register").asCom;
            _registerUIComponent.Center();
            Assert.IsNotNull(_registerUIComponent);

            _registerUIBackButton = _registerUIComponent.GetChild("Button_Back").asButton;
            _registerButton = _registerUIComponent.GetChild("Button_Register").asButton;
            _registerUITextInput = _registerUIComponent.GetChild("name").asTextInput;

            Assert.IsNotNull(_registerUIBackButton);
            Assert.IsNotNull(_registerButton);
            Assert.IsNotNull(_registerUITextInput);

            _window = new Window
            {
                contentPane = _registerUIComponent,
                closeButton = _registerUIBackButton,
                modal = true
            };

            _registerButton.onClick.Add((() => Register(tipPanel)));
        }

        public void Show()
        {
            _window.Show();
            MyWebSocket.MyWebSocket.Instance.Connect();
        }

        private void Register(TipPanel tipPanel)
        {
            if (_registerUITextInput.text.Equals(string.Empty))
            {
                tipPanel.Show("请输入昵称");
                return;
            }

            if (MyWebSocket.MyWebSocket.Instance.WebSocket.State != WebSocketStates.Open)
            {
                tipPanel.ShowNetWorkError();
                return;
            }

            var requestRegister = new RequestRegister(SystemInfo.deviceUniqueIdentifier, _registerUITextInput.text);
            requestRegister.RequestSuccess += () => { tipPanel.Show("注册成功"); };
            requestRegister.RequestFail += () => { tipPanel.Show("注册失败"); };

            MyGameManager.Instance.SendRequest(requestRegister);
        }
    }
}