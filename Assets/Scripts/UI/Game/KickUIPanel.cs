using FairyGUI;
using Manager;
using UI.Util;

namespace UI.Game
{
    public class KickUIPanel
    {
        public Window KickUIPanelWindow;

        private GComponent _kickPanelCom;
        private readonly GButton _talkButton;
        private GButton _voteButton;
        private GList _listPlayer;

        public KickUIPanel()
        {
            _kickPanelCom = UIPackage.CreateObject("Game", "KickPanel").asCom;
            KickUIPanelWindow = new Window
            {
                contentPane = _kickPanelCom,
                modal = true,
            };

            _talkButton = _kickPanelCom.GetChild("Button_Talk").asButton;
            _voteButton = _kickPanelCom.GetChild("Button_Vote").asButton;
            _listPlayer = _kickPanelCom.GetChild("List_Player").asList;
            _talkButton.onTouchBegin.Add(StartVoice);
            _talkButton.onTouchEnd.Add(EndVoice);
        }

        public void ShowPanel()
        {
            _listPlayer.RemoveChildren();
            AddPlayerNodeToList();
            KickUIPanelWindow.Show();
        }

        private void StartVoice()
        {
            _talkButton.title = "语音中";
            MyGameManager.Instance.localPlayerNetwork.ChangeVoiceIconShow(true);
            MyGameManager.Instance.VoiceStartTalk();
        }

        private void EndVoice()
        {
            _talkButton.title = "按住语音";
            MyGameManager.Instance.localPlayerNetwork.ChangeVoiceIconShow(false);
            MyGameManager.Instance.VoiceEndTalk();
        }

        private void AddPlayerNodeToList()
        {
            foreach (var playerController in MyGameManager.Instance.allPlayers)
            {
                if (playerController.isDead)
                {
                    continue;
                }

                var node = _listPlayer.AddChild(UIPackage.CreateObject("Game", "ReportListNode").asCom).asCom;
                node.GetChild("Text_User").asTextField.templateVars["accountName"] = playerController.playerAccountName;
            }
        }
    }
}