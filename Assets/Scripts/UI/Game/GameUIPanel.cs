using System;
using FairyGUI;
using Manager;
using UI.Util;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

namespace UI.Game
{
    public class GameUIPanel : UIPanelUtil
    {
        [Header("小地图渲染器纹理")] public Texture miniMapRanderTexture;

        private JoyStickModule _joystick;
        private GLoader _miniMapLoader;
        private GButton _reportButton;
        private GButton _killButton;
        private GButton _taskButton;
        private GProgressBar _gameProgress;

        protected override void Awake()
        {
            base.Awake();
            _joystick = new JoyStickModule(UIRoot);
            _joystick.onMove.Add(JoystickMove);
            _joystick.onEnd.Add(JoystickMove);

            _miniMapLoader = UIRoot.GetChild("Loader_MiniMap").asLoader;
            _reportButton = GetButton("Button_Report");
            _killButton = GetButton("Button_Kill");
            _taskButton = GetButton("Button_Task");
            CreatePlayer();
        }

        private void Start()
        {
            _miniMapLoader.texture = new NTexture(miniMapRanderTexture);
            _reportButton.onClick.Add(MyGameManager.Instance.localPlayerController.Report);
            _killButton.onClick.Add(MyGameManager.Instance.localPlayerController.KillTarget);
            _taskButton.onClick.Add(MyGameManager.Instance.localPlayerController.DoTask);
        }

        private static void JoystickMove(EventContext context)
        {
            MyGameManager.Instance.SendJoyStickDegreeToPlayers((JoyStickOutputXY)context.data);
        }

        private void CreatePlayer()
        {
            if (MyGameManager.Instance.allPlayers.FindIndex(controller =>
                    ReferenceEquals(controller, MyGameManager.Instance.localPlayerController)) == 0)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }
        }
    }
}