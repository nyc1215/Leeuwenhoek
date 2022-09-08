using FairyGUI;
using Manager;
using UI.Util;

namespace UI.Game
{
    public class KickUIPanel
    {
        public Window KickUIPanelWindow;

        private readonly GButton _talkButton;
        private GList _listPlayer;
        private string _titleState;

        public KickUIPanel()
        {
            KickUIPanelWindow = new Window
            {
                contentPane = UIPackage.CreateObject("Boot", "ChoosePanel").asCom,
                modal = true,
            };

            _talkButton = KickUIPanelWindow.contentPane.GetChild("Button_Talk").asButton;
            _listPlayer = KickUIPanelWindow.contentPane.GetChild("List_Player").asList;
            _titleState = KickUIPanelWindow.contentPane.GetChild("title").asTextField.templateVars["state"];
            _talkButton.onTouchBegin.Add(StartVoice);
            _talkButton.onTouchEnd.Add(EndVoice);

            AddPlayerNodeToList();
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