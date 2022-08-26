using System;
using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Manager;
using UI.Util;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Game
{
    public class GameUIPanel : UIPanelUtil
    {
        private JoyStickModule _joystick;

        private GButton _startServer;
        private GButton _startHost;
        private GButton _startClient;


        protected override void Awake()
        {
            base.Awake();
            _joystick = new JoyStickModule(UIRoot);
            _joystick.onMove.Add(JoystickMove);

            _startServer = GetButton("Button_Server");
            _startHost = GetButton("Button_Host");
            _startClient = GetButton("Button_Client");

            _startServer.onClick.Add(() => { NetworkManager.Singleton.StartServer(); });
            _startHost.onClick.Add(() => { NetworkManager.Singleton.StartHost(); });
            _startClient.onClick.Add(() => { NetworkManager.Singleton.StartClient(); });
        }

        private static void JoystickMove(EventContext context)
        {
            MyGameManager.Instance.SendJoyStickDegreeToPlayers((JoyStickOutputXY)context.data);
        }
    }
}