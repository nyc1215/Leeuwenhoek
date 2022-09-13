using System;
using System.Collections;
using System.Globalization;
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

        private WaitForSeconds _waitForASecond;

        public KickUIPanel KickUIPanel;

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
            _timer = UIRoot.GetChild("Text_Timer").asTextField;
            _waitForASecond = new WaitForSeconds(1);

            MyGameNetWorkManager.Instance.GameProgressBar = _gameProgress;

            KickUIPanel = new KickUIPanel();
        }

        private void Start()
        {
            _miniMapLoader.texture = new NTexture(miniMapRenderTexture);
            _reportButton.onClick.Add(MyGameManager.Instance.localPlayerController.Report);
            _killButton.onClick.Add(() =>
            {
                StartCoroutine(
                    ButtonCold(_killButton, 25f, MyGameManager.Instance.localPlayerController.KillTarget));
            });
            _taskButton.onClick.Add(MyGameManager.Instance.localPlayerController.DoTask);
            _sewerButton.onClick.Add(() =>
            {
                MyGameManager.Instance.localPlayerController.nowSewer.GoOtherSewer(MyGameManager.Instance
                    .localPlayerController);
            });


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


            StartCoroutine(TimeCountDown());
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
            var buttonIconImage = button.GetChild("icon").asImage;
            if (buttonIconImage.fillAmount >= 0.99f)
            {
                buttonDo.Invoke();
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

            yield return null;
        }

        private IEnumerator TimeCountDown()
        {
            while (MyGameManager.Instance.allTime > 0)
            {
                _timer.SetVar("Min",
                        Math.Floor(MyGameManager.Instance.allTime / 60f).ToString(CultureInfo.InvariantCulture))
                    .SetVar("Sec",
                        Math.Floor(MyGameManager.Instance.allTime % 60f).ToString("00", CultureInfo.InvariantCulture))
                    .FlushVars();
                yield return _waitForASecond;
                MyGameManager.Instance.allTime--;
            }
        }
    }
}