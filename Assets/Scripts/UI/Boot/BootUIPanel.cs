﻿using System;
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
        private GButton _registerButton;
        private GButton _loginButton;
        private GTextField _severState;

        public static TipPanel TipPanel;
        public static MatchingPanel MatchingPanel;
        private static RegisterPanel _registerPanel;
        private static LoginPanel _loginPanel;

        protected override void Awake()
        {
            base.Awake();

            TipPanel ??= new TipPanel();
            MatchingPanel ??= new MatchingPanel();
            _loginPanel ??= new LoginPanel();
            _registerPanel ??= new RegisterPanel();
        }

        private void Start()
        {
            _quitButton = GetButton("Button_Quit");
            _registerButton = GetButton("Button_Register");
            _loginButton = GetButton("Button_Login");
            _severState = UIRoot.GetChild("connect").asTextField;

            _quitButton?.onClick.Add(() => { StartCoroutine(QuitGame()); });
            _registerButton?.onClick.Add(() => { _registerPanel.Show(); });
            _loginButton?.onClick.Add(() => { _loginPanel.Show(); });
        }

        private static IEnumerator QuitGame()
        {
            if (MyWebSocket.MyWebSocket.Instance.WebSocket == null)
            {
                UIOperationUtil.QuitGame();
                yield break;
            }

            MyWebSocket.MyWebSocket.Instance.Close();
            while (MyWebSocket.MyWebSocket.Instance.WebSocket != null)
            {
                yield return null;
            }

            UIOperationUtil.QuitGame();
        }

        private void Update()
        {
            if (MyWebSocket.MyWebSocket.Instance.WebSocket == null)
            {
                return;
            }

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