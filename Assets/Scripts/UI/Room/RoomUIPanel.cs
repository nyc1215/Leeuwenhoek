using System.Linq;
using FairyGUI;
using Manager;
using MyWebSocket.Request;
using MyWebSocket.Response;
using UI.Game;
using UI.Util;
using UnityEngine;
using NetworkManager = Unity.Netcode.NetworkManager;

namespace UI.Room
{
    public class RoomUIPanel : UIPanelUtil
    {
        private JoyStickModule _joystick;

        private GButton _roomUIBackButton;
        private GButton _readyButton;
        private GButton _voiceButton;

        private int _localPlayerIndex;

        protected override void Awake()
        {
            base.Awake();
            _joystick = new JoyStickModule(UIRoot);
            _joystick.onMove.Add(JoystickMove);
            _joystick.onEnd.Add(JoystickMove);
        }

        private void Start()
        {
            ListUpdate();

            _roomUIBackButton = GetButton("Button_Back");
            _readyButton = GetButton("Button_Ready");
            _voiceButton = GetButton("Button_Voice");

            _roomUIBackButton.onClick.Add(PlayerExitRoom);
            _readyButton.onClick.Add(PlayerReady);
            _voiceButton.onTouchBegin.Add(StartVoice);
            _voiceButton.onTouchEnd.Add(EndVoice);

            CreatePlayer();

            MyGameManager.Instance.VoiceChangeRemoteVoice(false);
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
                _roomUIBackButton.touchable = false;
                _roomUIBackButton.GetChild("icon").asImage.color = new Color(0.4f, 0.4f, 0.4f);
                MyGameManager.Instance.NetWorkOperations.SendRequest(new RequestReady(
                    MyGameManager.Instance.LocalPlayerInfo.Account, MyGameManager.Instance.LocalPlayerInfo.GroupId));
            }
            else
            {
                MyGameManager.Instance.LocalPlayerInfo.ReadyForGame = false;
                _readyButton.text = "准备!";
                _roomUIBackButton.touchable = true;
                _roomUIBackButton.GetChild("icon").asImage.color = Color.white;
                MyGameManager.Instance.NetWorkOperations.SendRequest(new RequestCancelReady(
                    MyGameManager.Instance.LocalPlayerInfo.Account, MyGameManager.Instance.LocalPlayerInfo.GroupId));
            }

            MyGameManager.Instance.localPlayerNetwork.ChangeTopTextColor(MyGameManager.Instance.LocalPlayerInfo
                .ReadyForGame);
        }

        private static void PlayerExitRoom()
        {
            MyGameManager.Instance.VoiceLeaveChannel();
            var requestExitGroup = new RequestExitGroup(
                MyGameManager.Instance.LocalPlayerInfo.Account, MyGameManager.Instance.LocalPlayerInfo.GroupId);
            MyGameManager.Instance.NetWorkOperations.SendRequest(requestExitGroup);
            MyGameManager.Instance.localPlayerController.DestroyPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
            UIOperationUtil.GoToScene(MyGameManager.Instance.uiJumpData.bootMenu);
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
    }
}