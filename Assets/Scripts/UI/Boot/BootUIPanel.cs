using System;
using System.Collections;
using BestHTTP.WebSocket;
using FairyGUI;
using Manager;
using Mirror;
using UI.Util;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace UI.Boot
{
    /// <summary>
    /// 启动界面UI
    /// </summary>
    public class BootUIPanel : UIPanelUtil
    {
        private GButton _quitButton;
        private GButton _registButton;
        private GButton _loginButton;
        private GTextField _severState;

        private void Start()
        {
            _quitButton = GetButton("Button_Quit");
            _registButton = GetButton("Button_Regist");
            _loginButton = GetButton("Button_Login");
            _severState = UIRoot.GetChild("connect").asTextField;

            _quitButton?.onClick.Add((() => { StartCoroutine(QuitGame()); }));
            _registButton?.onClick.Add(() =>
            {
                RegistPanel.RegistPanelInit(UIRoot);
                RegistPanel.Show();
            });
            _loginButton?.onClick.Add((() =>
            {
                LoginPanel.LoginPanelInit(UIRoot);
                LoginPanel.Show();
            }));
            
            TipPanel.TipPanelInit(UIRoot);
        }

        private static IEnumerator QuitGame()
        {
            if (MyWebSocket.MyWebSocket.Instance.WebSocket == null)
            {
                UIOperationUtil.QuitGame();
                yield break;
            }

            MyWebSocket.MyWebSocket.Instance.Close();
            while (MyWebSocket.MyWebSocket.Instance.WebSocket.State != WebSocketStates.Closed)
            {
                yield return null;
            }

            UIOperationUtil.QuitGame();
        }

        private void Update()
        {
            if (MyWebSocket.MyWebSocket.Instance.WebSocket != null)
            {
                switch (MyWebSocket.MyWebSocket.Instance.WebSocket.State)
                {
                    case WebSocketStates.Connecting:
                        _severState.SetVar("isConnect", "连接中").FlushVars();
                        break;
                    case WebSocketStates.Open:
                        _severState.SetVar("isConnect", "已连接").FlushVars();
                        break;
                    case WebSocketStates.Closed:
                    case WebSocketStates.Closing:
                    case WebSocketStates.Unknown:
                        _severState.SetVar("isConnect", "未连接").FlushVars();
                        break;
                    default:
                        _severState.SetVar("isConnect", "未连接").FlushVars();
                        break;
                }
               
            }
        }
    }
}