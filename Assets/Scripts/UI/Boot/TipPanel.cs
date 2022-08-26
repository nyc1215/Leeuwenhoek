using System.Collections;
using System.Collections.Generic;
using BestHTTP.WebSocket;
using UnityEngine;
using FairyGUI;
using UnityEngine.Assertions;

namespace UI.Boot
{
    public class TipPanel
    {
        private readonly Window _window;
        private readonly GComponent _tipUIComponent;
        private readonly GButton _tipUIBackButton;
        private readonly GTextField _tipUIText;

        /// <summary>
        /// 提示UI的初始化
        /// </summary>
        public TipPanel()
        {
            _tipUIComponent = UIPackage.CreateObject("Boot", "Tip").asCom;
            _tipUIComponent.Center();
            Assert.IsNotNull(_tipUIComponent);

            _tipUIBackButton = _tipUIComponent.GetChild("Button_Back").asButton;
            _tipUIText = _tipUIComponent.GetChild("Title").asTextField;

            Assert.IsNotNull(_tipUIBackButton);
            Assert.IsNotNull(_tipUIText);


            _window = new Window
            {
                contentPane = _tipUIComponent,
                closeButton = _tipUIBackButton,
                modal = true
            };
        }

        public void ShowNetWorkError()
        {
            switch (MyWebSocket.MyWebSocket.WebSocket.State)
            {
                case WebSocketStates.Connecting:
                    ShowConnecting();
                    break;
                case WebSocketStates.Open:
                    ShowSuccess();
                    break;
                case WebSocketStates.Closed:
                case WebSocketStates.Closing:
                case WebSocketStates.Unknown:
                    ShowFail();
                    break;
                default:
                    ShowFail();
                    break;
            }

            _window.Show();
            _window.BringToFront();
        }

        public void Show(string tips)
        {
            if (string.IsNullOrEmpty(tips))
            {
                return;
            }
            
            _tipUIText.text = tips;
            _window.Show();
            _window.BringToFront();
        }

        /// <summary>
        ///连接中的显示
        /// </summary>
        private void ShowConnecting()
        {
            _tipUIText.text = "连接中";
        }

        /// <summary>
        ///连接成功的显示
        /// </summary>
        private void ShowSuccess()
        {
            _tipUIText.text = "服务器连接成功";
        }

        /// <summary>
        ///连接失败的显示
        /// </summary>
        private void ShowFail()
        {
            _tipUIText.text = "服务器连接失败";
        }
    }
}