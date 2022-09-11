using System;
using System.Linq;
using FairyGUI;
using Manager;
using MyWebSocket.Request;
using MyWebSocket.Response;
using UI.Game;
using UI.Util;
using UnityEngine;
using UnityEngine.Serialization;
using NetworkManager = Unity.Netcode.NetworkManager;

namespace UI.Room
{
    public class RoomUIPanel : UIPanelUtil
    {
        public RoomReadyStory roomReadyStory;

        private GComponent _storyCom;
        private GComponent _characterCom;
        private GTextField _storyText;
        private GTextField _storyLetter;
        private GTextField _storyLetterName;
        private GLoader _storyImg;
        private int _storyIndex;

        private JoyStickModule _joystick;

        private GButton _readyButton;
        private GButton _voiceButton;

        private int _localPlayerIndex;

        protected override void Awake()
        {
            base.Awake();
            _joystick = new JoyStickModule(UIRoot);
            _joystick.onMove.Add(JoystickMove);
            _joystick.onEnd.Add(JoystickMove);

            _characterCom = UIPackage.CreateObject("Room", "CharacterPanel").asCom;

            _storyIndex = 0;
            _storyCom = UIRoot.AddChild(UIPackage.CreateObject("Room", "StoryPanel")).asCom;
            _storyImg = _storyCom.GetChild("Loader_Img").asLoader;
            _storyText = _storyCom.GetChild("Text_Story").asTextField;
            _storyLetter = _storyCom.GetChild("Text_Letter").asTextField;
            _storyLetter.SetVar("AccountName", MyGameManager.Instance.LocalPlayerInfo.AccountName).FlushVars();
            _storyLetterName = _storyCom.GetChild("Text_LetterName").asTextField;
        }

        private void Start()
        {
            ListUpdate();

            _readyButton = GetButton("Button_Ready");
            _voiceButton = GetButton("Button_Voice");

            _readyButton.onClick.Add(PlayerReady);
            _voiceButton.onTouchBegin.Add(StartVoice);
            _voiceButton.onTouchEnd.Add(EndVoice);

            CreatePlayer();
            MyGameManager.Instance.VoiceChangeRemoteVoice(false);

            _storyCom.onClick.Add(OnStoryClicked);
            _storyText.text = roomReadyStory.storyText[_storyIndex];

            _characterCom.onClick.Add(() => { _characterCom.Dispose(); });
        }

        public void ListUpdate()
        {
            for (var i = 0; i < MyGameManager.Instance.PlayerListData.PlayerList.Count; i++)
            {
                var playerListNode = MyGameManager.Instance.PlayerListData.PlayerList[i];

                if (playerListNode.Account == MyGameManager.Instance.LocalPlayerInfo.Account)
                {
                    _localPlayerIndex = i;
                    MyGameManager.Instance.LocalPlayerInfo.AccountName = playerListNode.AccountName;
                }
            }
        }

        private void PlayerReady()
        {
            if (!MyGameManager.Instance.LocalPlayerInfo.ReadyForGame)
            {
                MyGameManager.Instance.LocalPlayerInfo.ReadyForGame = true;
                _readyButton.text = "已准备";
                MyGameManager.Instance.NetWorkOperations.SendRequest(new RequestReady(
                    MyGameManager.Instance.LocalPlayerInfo.Account, MyGameManager.Instance.LocalPlayerInfo.GroupId));
            }
            else
            {
                MyGameManager.Instance.LocalPlayerInfo.ReadyForGame = false;
                _readyButton.text = "准备!";
                MyGameManager.Instance.NetWorkOperations.SendRequest(new RequestCancelReady(
                    MyGameManager.Instance.LocalPlayerInfo.Account, MyGameManager.Instance.LocalPlayerInfo.GroupId));
            }

            MyGameManager.Instance.localPlayerNetwork.ChangeTopTextColor(MyGameManager.Instance.LocalPlayerInfo
                .ReadyForGame);
        }

        public void CheckGameStart(PlayerRoomStatusData playerRoomStatusData)
        {
            foreach (var playerListNode in MyGameManager.Instance.PlayerListData.PlayerList.Where(playerListNode =>
                         playerListNode.Account == playerRoomStatusData.Account))
            {
                playerListNode.Status = playerRoomStatusData.Status;
                ListUpdate();
                break;
            }

            var gameStart =
                MyGameManager.Instance.PlayerListData.PlayerList.All(playerListNode =>
                    playerListNode.Status == "READY");

            if (gameStart)
            {
                MyGameManager.Instance.GameStart();
            }
        }

        private static void JoystickMove(EventContext context)
        {
            MyGameManager.Instance.SendJoyStickDegreeToPlayers((JoyStickOutputXY)context.data);
        }

        private void CreatePlayer()
        {
            if (_localPlayerIndex == 0)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }
        }

        private void StartVoice()
        {
            _voiceButton.title = "语音中";
            MyGameManager.Instance.localPlayerNetwork.ChangeVoiceIconShow(true);
            MyGameManager.Instance.VoiceStartTalk();
        }

        private void EndVoice()
        {
            _voiceButton.title = "按住语音";
            MyGameManager.Instance.localPlayerNetwork.ChangeVoiceIconShow(false);
            MyGameManager.Instance.VoiceEndTalk();
        }


        private void OnStoryClicked()
        {
            if (_storyIndex == 16)
            {
                _storyIndex += MyGameManager.Instance.localPlayerController.isImposter ? 2 : 1;
            }
            else
            {
                _storyIndex++;
            }

            if (MyGameManager.Instance.localPlayerController != null &&
                MyGameManager.Instance.localPlayerController.isImposter)
            {
                if (_storyIndex >= 19)
                {
                    _storyCom.onClick.Clear();
                    _storyCom.Dispose();
                    return;
                }
            }
            else
            {
                if (_storyIndex >= 18)
                {
                    _storyCom.onClick.Clear();
                    _storyCom.Dispose();
                    return;
                }
            }

            _storyText.text = roomReadyStory.storyText[_storyIndex];

            if (_storyIndex is >= 1 and <= 2)
            {
                _storyImg.url = "ui://Room/序章剧本1";
            }
            else if (_storyIndex is >= 3 and <= 6)
            {
                _storyImg.url = "ui://Room/序章剧本2";
            }
            else if (_storyIndex == 9)
            {
                _storyImg.url = null;
                _storyLetter.visible = true;
                _storyLetterName.visible = true;
                _storyText.visible = false;
            }
            else if (_storyIndex == 12)
            {
                UIRoot.AddChild(_characterCom);
            }
            else if (_storyIndex is >= 13 and <= 18)
            {
                _storyImg.url = "ui://Room/序章剧本3";
            }
            else
            {
                _storyLetter.visible = false;
                _storyLetterName.visible = false;
                _storyText.visible = true;
                _storyImg.url = null;
            }
        }
    }
}