using System.Collections;
using System.Collections.Generic;
using BestHTTP.WebSocket;
using UnityEngine;
using FairyGUI;
using UnityEngine.Assertions;

namespace UI.Boot
{
    public static class TipPanel
    {
        private static GComponent _tipUIComponent;
        private static GButton _tipUIBackButton;
        private static GTextField _tipUIText;

        /// <summary>
        /// 提示UI的初始化
        /// </summary>
        public static void TipPanelInit(GComponent uiRoot)
        {
            _tipUIComponent = UIPackage.CreateObject("Boot", "Tip").asCom;
            _tipUIComponent.Center();
            _tipUIComponent.sortingOrder = 10;
            Assert.IsNotNull(_tipUIComponent);

            _tipUIBackButton = _tipUIComponent.GetChild("Button_Back").asButton;
            _tipUIText = _tipUIComponent.GetChild("Title").asTextField;

            Assert.IsNotNull(_tipUIBackButton);
            Assert.IsNotNull(_tipUIText);

            _tipUIComponent.visible = false;
            _tipUIBackButton.onClick.Add((() => { _tipUIComponent.visible = false; }));

            GRoot.inst.AddChild(_tipUIComponent);
        }

        public static void ShowNetWorkError()
        {
            _tipUIComponent.visible = true;
            switch (MyWebSocket.MyWebSocket.Instance.WebSocket.State)
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
        }

        public static void Show(string tips)
        {
            if (!string.IsNullOrEmpty(tips))
            {
                _tipUIText.text = tips;
                _tipUIComponent.visible = true;
            }
        }

        /// <summary>
        ///连接中的显示
        /// </summary>
        private static void ShowConnecting()
        {
            _tipUIText.text = "连接中";
        }

        /// <summary>
        ///连接成功的显示
        /// </summary>
        private static void ShowSuccess()
        {
            _tipUIText.text = "服务器连接成功";
        }

        /// <summary>
        ///连接失败的显示
        /// </summary>
        private static void ShowFail()
        {
            _tipUIText.text = "服务器连接失败";
        }
    }
}