using System.Collections.Generic;
using FairyGUI;
using Manager;
using UI.Util;
using UnityEngine;

namespace UI.Game
{
    public class KickUIPanel
    {
        public readonly Window KickUIPanelWindow;

        private readonly Dictionary<Characters, string> _characterImgURL;

        private readonly GComponent _kickPanelCom;
        private readonly GButton _talkButton;
        private readonly GButton _abandonVoteButton;
        private readonly GList _listPlayer;
        private readonly GTextField _timer;
        private readonly GTextField _selectWho;

        private Characters _lastVoteCharacter;

        public KickUIPanel()
        {
            _kickPanelCom = UIPackage.CreateObject("Game", "KickPanel").asCom;
            KickUIPanelWindow = new Window
            {
                contentPane = _kickPanelCom,
                modal = true,
            };

            _talkButton = _kickPanelCom.GetChild("Button_Talk").asButton;
            _talkButton.onTouchBegin.Add(StartVoice);
            _talkButton.onTouchEnd.Add(EndVoice);

            _abandonVoteButton = _kickPanelCom.GetChild("Button_AbandonVote").asButton;
            _abandonVoteButton.onClick.Add(AbandonVote);

            _listPlayer = _kickPanelCom.GetChild("List_Player").asList;

            _timer = _kickPanelCom.GetChild("title").asTextField;
            _selectWho = _kickPanelCom.GetChild("Text_selectWho").asTextField;

            _characterImgURL = new Dictionary<Characters, string>
            {
                { Characters.Lily, "ui://o5dbpgr6rqoe11" },
                { Characters.None, "" },
                { Characters.Polo, "ui://o5dbpgr6di36y" },
                { Characters.Xuela, "ui://o5dbpgr6di3610" },
                { Characters.Yang, "ui://o5dbpgr6rqoe13" },
                { Characters.LuoWei, "ui://o5dbpgr6rqoe12" },
                { Characters.XiaoAn, "ui://o5dbpgr6di36z" }
            };

            _lastVoteCharacter = Characters.None;
        }

        public void ShowPanel()
        {
            _lastVoteCharacter = Characters.None;
            _listPlayer.RemoveChildren();
            AddPlayerNodeToList();
            MyGameNetWorkManager.Instance.InitVoteServerRpc();
            KickUIPanelWindow.Show();
            MyGameNetWorkManager.Instance.StartReportCountDown();
        }

        private void ClosePanel()
        {
            MyGameManager.Instance.VoiceEndTalk();
            KickUIPanelWindow.Hide();
            MyGameManager.Instance.localPlayerController.OnEnable();
        }

        private void StartVoice()
        {
            if (MyGameManager.Instance.localPlayerController.isDead || MyGameManager.Instance.localPlayerController.isKicked)
            {
                return;
            }

            _talkButton.title = "语音中";
            MyGameNetWorkManager.Instance.ChangeVoteTextServerRpc(MyGameManager.Instance.localPlayerController.nowCharacter, true);
            MyGameManager.Instance.VoiceStartTalk();
        }

        private void EndVoice()
        {
            if (MyGameManager.Instance.localPlayerController.isDead || MyGameManager.Instance.localPlayerController.isKicked)
            {
                return;
            }

            _talkButton.title = "按住语音";
            MyGameNetWorkManager.Instance.ChangeVoteTextServerRpc(MyGameManager.Instance.localPlayerController.nowCharacter, false);
            MyGameManager.Instance.VoiceEndTalk();
        }

        private void AddPlayerNodeToList()
        {
            foreach (var playerState in MyGameNetWorkManager.Instance.NetLobbyPlayersCharacterStates)
            {
                var node = _listPlayer.AddChild(UIPackage.CreateObject("Game", "ReportListNode").asCom).asCom;
                node.GetChild("Loader_head").asLoader.url = _characterImgURL[playerState.CharacterToChoose];

                var nodeText = node.GetChild("Text_User").asTextField;
                if (playerState.IsDead || playerState.IsKicked)
                {
                    node.touchable = false;
                    nodeText.text = "{accountName=user} ×";
                    nodeText.color = Color.red;
                    nodeText.SetVar("accountName", playerState.AccountName.ToString()).FlushVars();
                }
                else
                {
                    nodeText.SetVar("accountName", playerState.AccountName.ToString()).SetVar("vote", playerState.Vote.ToString()).FlushVars();
                    node.onClick.Add(() => { Vote(playerState.CharacterToChoose, nodeText.templateVars["accountName"]); });
                }
            }
        }

        public void UpdateVoteState()
        {
            for (var i = 0; i < MyGameNetWorkManager.Instance.NetLobbyPlayersCharacterStates.Count; i++)
            {
                _listPlayer.GetChildAt(i).asCom.GetChild("Text_User").asTextField.SetVar("vote", MyGameNetWorkManager.Instance.NetLobbyPlayersCharacterStates[i].Vote.ToString()).FlushVars();
                _listPlayer.GetChildAt(i).asCom.GetChild("Text_Talk").visible = MyGameNetWorkManager.Instance.NetLobbyPlayersCharacterStates[i].IsTalking;
            }
        }

        public void UpdateCountDown(int nowTime)
        {
            _timer.SetVar("state", nowTime.ToString()).FlushVars();
        }

        public void CountDownFinish()
        {
            MyGameNetWorkManager.Instance.KickSomeOne();
            MyGameNetWorkManager.Instance.StopReportCountDown();
            ClosePanel();
        }

        private void AbandonVote()
        {
            if (MyGameManager.Instance.localPlayerController.isDead || MyGameManager.Instance.localPlayerController.isKicked)
            {
                return;
            }

            if (_lastVoteCharacter != Characters.None && _lastVoteCharacter != MyGameManager.Instance.localPlayerController.nowCharacter)
            {
                MyGameNetWorkManager.Instance.ChangeVoteServerRpc(_lastVoteCharacter, false);
                _lastVoteCharacter = Characters.None;
                _selectWho.text = "放弃投票";
            }
        }

        private void Vote(Characters character, string accountName)
        {
            if (MyGameManager.Instance.localPlayerController.isDead || MyGameManager.Instance.localPlayerController.isKicked)
            {
                _selectWho.text = "无法投票";
                return;
            }

            if (_lastVoteCharacter != Characters.None)
            {
                MyGameNetWorkManager.Instance.ChangeVoteServerRpc(_lastVoteCharacter, false);
            }

            if (character == MyGameManager.Instance.localPlayerController.nowCharacter)
            {
                _lastVoteCharacter = Characters.None;
                _selectWho.text = "放弃投票";
                return;
            }

            MyGameNetWorkManager.Instance.ChangeVoteServerRpc(character, true);
            _lastVoteCharacter = character;
            _selectWho.text = "你把票投给了:{who=}";
            _selectWho.SetVar("who", accountName).FlushVars();
        }
    }
}