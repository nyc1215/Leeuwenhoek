using System;
using System.Collections;
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
        [Header("小地图渲染器纹理")] public Texture miniMapRenderTexture;

        private JoyStickModule _joystick;
        private GLoader _miniMapLoader;
        private GButton _reportButton;
        private GButton _killButton;
        private GButton _taskButton;
        private GButton _sewerButton;
        private GProgressBar _gameProgress;
        private GTextField _timer;

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
            _sewerButton = GetButton("Button_Sewer");

            _gameProgress = UIRoot.GetChild("ProgressBar_Game").asProgress;

            MyGameNetWorkManager.Instance.GameProgressBar = _gameProgress;
        }

        private void Start()
        {
            _miniMapLoader.texture = new NTexture(miniMapRenderTexture);
            _reportButton.onClick.Add(MyGameManager.Instance.localPlayerController.Report);
            _killButton.onClick.Add(() => { StartCoroutine(ButtonCold(_killButton, 25f, MyGameManager.Instance.localPlayerController.KillTarget)); });
            _taskButton.onClick.Add(MyGameManager.Instance.localPlayerController.DoTask);
            _sewerButton.onClick.Add(() =>
            {
                MyGameManager.Instance.localPlayerController.nowSewer.GoOtherSewer(MyGameManager.Instance
                    .localPlayerController);
            });

            if (MyGameManager.Instance.localPlayerController.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                if (MyGameManager.Instance.localPlayerController.isImposter)
                {
                    _sewerButton.visible = false;
                    _killButton.visible = true;
                }
                else
                {
                    _sewerButton.visible = false;
                    _killButton.visible = false;
                }
            }
        }

        private static void JoystickMove(EventContext context)
        {
            MyGameManager.Instance.SendJoyStickDegreeToPlayers((JoyStickOutputXY)context.data);
        }

        public void ChangeSewerButtonVisible(bool isVisible)
        {
            _sewerButton.visible = isVisible;
        }

        public void ChangeTaskButtonVisible(bool isVisible)
        {
            _taskButton.visible = isVisible;
        }


        private static IEnumerator ButtonCold(GButton button, float coldTime, Action buttonDo)
        {
            var buttonIconImage = button.GetChild(button.icon).asImage;
            if (buttonIconImage.fillAmount >= 0.99f)
            {
                button.touchable = false;
                buttonIconImage.fillAmount = 0f;
                var nowTime = coldTime;
                while (nowTime < coldTime)
                {
                    nowTime += Time.deltaTime;
                    buttonIconImage.fillAmount = nowTime / coldTime;
                    yield return null;
                }

                button.touchable = true;
            }
            else
            {
                buttonDo.Invoke();
            }

            yield return null;
        }
    }
}