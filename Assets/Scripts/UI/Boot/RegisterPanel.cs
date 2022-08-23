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
        private readonly GTextInput _registerAccountInput;
        private readonly GTextInput _registerAccountNameInput;

        /// <summary>
        /// UI的初始化
        /// </summary>
        public RegisterPanel()
        {
            _registerUIComponent = UIPackage.CreateObject("Boot", "Register").asCom;
            _registerUIComponent.Center();
            Assert.IsNotNull(_registerUIComponent);

            _registerUIBackButton = _registerUIComponent.GetChild("Button_Back").asButton;
            _registerButton = _registerUIComponent.GetChild("Button_Register").asButton;
            _registerAccountInput = _registerUIComponent.GetChild("Input_Account").asTextInput;
            _registerAccountNameInput = _registerUIComponent.GetChild("Input_Name").asTextInput;

            Assert.IsNotNull(_registerUIBackButton);
            Assert.IsNotNull(_registerButton);
            Assert.IsNotNull(_registerAccountNameInput);

            _window = new Window
            {
                contentPane = _registerUIComponent,
                closeButton = _registerUIBackButton,
                modal = true
            };

            _registerButton.onClick.Add(Register);
            _registerAccountInput.onFocusIn.Add(() => { _registerAccountInput.promptText = string.Empty; });
            _registerAccountNameInput.onFocusIn.Add(() => { _registerAccountNameInput.promptText = string.Empty; });
        }

        public void Show()
        {
            _window.Show();
            _registerAccountInput.promptText = "请输入账号";
            _registerAccountInput.text = string.Empty;
            _registerAccountNameInput.promptText = "请输入昵称";
            _registerAccountNameInput.text = string.Empty;
            MyWebSocket.MyWebSocket.Instance.Connect();
        }

        private void Register()
        {
            MyWebSocket.MyWebSocket.Instance.Connect();

            if (_registerAccountNameInput.text.Equals(string.Empty))
            {
                BootUIPanel.TipPanel.Show("请输入昵称");
                return;
            }

            if (MyWebSocket.MyWebSocket.Instance.WebSocket.State != WebSocketStates.Open)
            {
                BootUIPanel.TipPanel.ShowNetWorkError();
                return;
            }

            var requestRegister = new RequestRegister(_registerAccountInput.text, _registerAccountNameInput.text);
            requestRegister.RequestSuccess += () =>
            {
                BootUIPanel.TipPanel.Show("注册成功");
                _window.Hide();
            };
            requestRegister.RequestFail += () => { BootUIPanel.TipPanel.Show("注册失败"); };

            MyGameManager.Instance.NetWorkOperations.SendRequest(requestRegister);
        }
    }
}