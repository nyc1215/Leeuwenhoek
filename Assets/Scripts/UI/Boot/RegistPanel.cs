using System.Collections;
using System.Collections.Generic;
using BestHTTP.WebSocket;
using FairyGUI;
using MyWebSocket.Request;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Boot
{
    public static class RegistPanel
    {
        private static GComponent _registUIComponent;
        private static GButton _registUIBackButton;
        private static GButton _registButton;
        private static GTextInput _registUITextInput;

        /// <summary>
        /// 提示UI的初始化
        /// </summary>
        public static void RegistPanelInit(GComponent uiRoot)
        {
            _registUIComponent = UIPackage.CreateObject("Boot", "Regist").asCom;
            _registUIComponent.Center();
            Assert.IsNotNull(_registUIComponent);

            _registUIBackButton = _registUIComponent.GetChild("Button_Back").asButton;
            _registButton = _registUIComponent.GetChild("Button_Regist").asButton;
            _registUITextInput = _registUIComponent.GetChild("name").asTextInput;

            Assert.IsNotNull(_registUIBackButton);
            Assert.IsNotNull(_registButton);
            Assert.IsNotNull(_registUITextInput);

            _registUIComponent.visible = false;
            _registUIBackButton.onClick.Add((() => { _registUIComponent.visible = false; }));
            _registButton.onClick.Add(Regist);

            GRoot.inst.AddChild(_registUIComponent);
        }

        public static void Show()
        {
            _registUIComponent.visible = true;
            MyWebSocket.MyWebSocket.Instance.Connect();
        }

        private static void Regist()
        {
            if (_registUITextInput.text.Equals(string.Empty))
            {
                TipPanel.Show("请输入昵称");
                return;
            }

            if (MyWebSocket.MyWebSocket.Instance.WebSocket.State != WebSocketStates.Open)
            {
                TipPanel.ShowNetWorkError();
                return;
            }

            MyWebSocket.MyWebSocket.Instance.Send("123456");
            MyWebSocket.MyWebSocket.Instance.Send(new RequestRegister("1", _registUITextInput.text).ToJson());
        }
    }
}