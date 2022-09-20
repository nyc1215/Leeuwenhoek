using System.Linq;
using FairyGUI;
using Manager;
using MyWebSocket.Request;
using MyWebSocket.Response;
using UI.Game;
using UI.Util;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using NetworkManager = Unity.Netcode.NetworkManager;

namespace UI.Room
{
    public class RoomUIPanel : UIPanelUtil
    {
        public RoomReadyStory roomReadyStory;
        public AudioClip knockDoorMusic;

        private GComponent _storyCom;
        private GTextField _storyText;
        private GTextField _storyLetter;
        private GTextField _storyLetterName;
        private GLoader _storyImg;
        private int _storyIndex;

        private JoyStickModule _joystick;

        private GButton _readyButton;
        private GButton _voiceButton;

        public CharacterPanel CharacterPanel;

        private TypingEffect _typingEffect;

        protected override void Awake()
        {
            base.Awake();
            _joystick = new JoyStickModule(UIRoot);
            _joystick.onMove.Add(JoystickMove);
            _joystick.onEnd.Add(JoystickMove);

            CharacterPanel = new CharacterPanel(UIRoot);

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

            MyGameManager.Instance.VoiceChangeRemoteVoice(false);

            _storyCom.onClick.Add(OnStoryClicked);
            if (MyGameManager.Instance.gameObject.TryGetComponent<AudioSource>(out var bgm))
            {
                bgm.PlayOneShot(knockDoorMusic);
            }

            _typingEffect = new TypingEffect(_storyText);
            _storyText.text = roomReadyStory.storyText[_storyIndex];
            _typingEffect.Start();
            Timers.inst.StartCoroutine(_typingEffect.Print(0.050f));

            CreatePlayer();
        }

        public void ListUpdate()
        {
            foreach (var playerListNode in MyGameManager.Instance.PlayerListData.PlayerList.Where(playerListNode =>
                         playerListNode.Account == MyGameManager.Instance.LocalPlayerInfo.Account))
            {
                MyGameManager.Instance.LocalPlayerInfo.AccountName = playerListNode.AccountName;
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

        private static void CreatePlayer()
        {
            if (MyGameManager.Instance.isServer)
            {
                var utpTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                if (utpTransport)
                {
                    utpTransport.ConnectionData.Address = MyGameManager.Instance.serverIP;
                    utpTransport.ConnectionData.ServerListenAddress = "0.0.0.0";
                }

                NetworkManager.Singleton.StartHost();
                if (utpTransport)
                {
                    Debug.Log(
                        $"host connect info: {utpTransport.ConnectionData.Address} {utpTransport.ConnectionData.Port} {utpTransport.ConnectionData.ServerListenAddress}");
                }
            }
            else
            {
                var utpTransport = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                if (utpTransport)
                {
                    utpTransport.ConnectionData.Address = MyGameManager.Instance.serverIP;
                    utpTransport.ConnectionData.ServerListenAddress = MyGameManager.Instance.serverIP;
                }

                NetworkManager.Singleton.StartClient();
                if (utpTransport)
                {
                    Debug.Log(
                        $"client connect info: {utpTransport.ConnectionData.Address} {utpTransport.ConnectionData.Port} {utpTransport.ConnectionData.ServerListenAddress}");
                }
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
            _typingEffect.Start();
            Timers.inst.StartCoroutine(_typingEffect.Print(0.050f));

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
                CharacterPanel.Show();
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