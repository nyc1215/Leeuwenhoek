using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using FairyGUI;
using FairyGUI.Utils;
using InteractiveObj;
using Manager;
using UI.Util;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;
using UnityEngine.Serialization;

namespace UI.Game
{
    public class GameUIPanel : UIPanelUtil
    {
        [Header("小地图渲染器纹理")] public Texture miniMapRenderTexture;
        [Header("%25开启线索")] public List<StoryWithProgress> storyWith25Progress = new();
        [Header("%40开启线索")] public List<StoryWithProgress> storyWith40Progress = new();
        [Header("%75开启线索")] public List<StoryWithProgress> storyWith75Progress = new();


        private JoyStickModule _joystick;
        private GLoader _miniMapLoader;
        private GButton _reportButton;
        private GButton _killButton;
        private GButton _taskButton;
        private GButton _sewerButton;
        private GButton _storyButton;
        private GProgressBar _gameProgress;
        private GTextField _timer;
        private GTextField _playerNum;

        private WaitForSeconds _waitForASecond;

        public KickUIPanel KickUIPanel;
        public GameTipPanel GameTipPanel;

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
            _storyButton = GetButton("Button_Log");

            _gameProgress = UIRoot.GetChild("ProgressBar_Game").asProgress;
            _timer = UIRoot.GetChild("Text_Timer").asTextField;
            _playerNum = UIRoot.GetChild("Text_PlayerNum").asTextField;
            _waitForASecond = new WaitForSeconds(1);

            MyGameNetWorkManager.Instance.GameProgressBar = _gameProgress;

            KickUIPanel = new KickUIPanel();
            GameTipPanel = new GameTipPanel();
        }

        private void Start()
        {
            _miniMapLoader.texture = new NTexture(miniMapRenderTexture);
            SetPlayerNumText(MyGameManager.Instance.PlayerListData.PlayerList.Count);

            _reportButton.onClick.Add(MyGameManager.Instance.localPlayerController.Report);
            _killButton.onClick.Add(() =>
            {
                StartCoroutine(
                    ButtonCold(_killButton, 25f, MyGameManager.Instance.localPlayerController.KillTarget));
            });
            _taskButton.onClick.Add(MyGameManager.Instance.localPlayerController.DoTask);
            _storyButton.onClick.Add(MyGameManager.Instance.localPlayerController.ShowStory);
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

            HideAllStories();
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

        public void ChangeStoryButtonVisible(bool isVisible)
        {
            _storyButton.visible = isVisible;
        }


        private static IEnumerator ButtonCold(GButton button, float coldTime, Action buttonDo)
        {
            var buttonIconImage = button.GetChild("icon").asImage;
            if (buttonIconImage.fillAmount >= 0.99f)
            {
                buttonDo.Invoke();
                button.touchable = false;
                MyGameManager.Instance.localPlayerController.inKillCold = true;
                buttonIconImage.fillAmount = 0f;
                var nowTime = 0f;
                while (nowTime < coldTime)
                {
                    nowTime += Time.deltaTime;
                    buttonIconImage.fillAmount = nowTime / coldTime;
                    yield return null;
                }

                MyGameManager.Instance.localPlayerController.inKillCold = false;
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

        public void HideAllButtons()
        {
            _reportButton.visible = false;
            _killButton.visible = false;
            _taskButton.visible = false;
            _sewerButton.visible = false;
        }

        private void HideAllStories()
        {
            foreach (var story in storyWith25Progress.Where(story => story != null))
            {
                story.gameObject.SetActive(false);
            }

            foreach (var story in storyWith40Progress.Where(story => story != null))
            {
                story.gameObject.SetActive(false);
            }

            foreach (var story in storyWith75Progress.Where(story => story != null))
            {
                story.gameObject.SetActive(false);
            }
        }

        public void ShowStories()
        {
            if (_gameProgress.value >= 25)
            {
                foreach (var story in storyWith25Progress.Where(story => story != null))
                {
                    story.gameObject.SetActive(true);
                }
            }
            else if (_gameProgress.value >= 40)
            {
                foreach (var story in storyWith40Progress.Where(story => story != null))
                {
                    story.gameObject.SetActive(true);
                }
            }
            else if (_gameProgress.value >= 75)
            {
                foreach (var story in storyWith75Progress.Where(story => story != null))
                {
                    story.gameObject.SetActive(true);
                }
            }
        }

        public void SetPlayerNumText(int num)
        {
            _playerNum.SetVar("num", num.ToString()).FlushVars();
        }
    }
}