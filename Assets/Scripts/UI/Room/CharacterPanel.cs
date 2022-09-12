using System.Linq;
using FairyGUI;
using Manager;
using Unity.Netcode;

namespace UI.Room
{
    public class CharacterPanel
    {
        private readonly GComponent _characterCom;
        private readonly GComponent _uiRoot;

        private GLoader _buttonLuowei;
        private GLoader _buttonYang;
        private GLoader _buttonPolo;
        private GLoader _buttonXiaoan;
        private GLoader _buttonLily;
        private GLoader _buttonXuela;

        private GTextField _textLuowei;
        private GTextField _textYang;
        private GTextField _textPolo;
        private GTextField _textXiaoan;
        private GTextField _textLily;
        private GTextField _textXuela;

        private GButton _buttonContinue;

        public CharacterPanel(GComponent uiRoot)
        {
            _uiRoot = uiRoot;

            _characterCom = UIPackage.CreateObject("Room", "CharacterPanel").asCom;

            _buttonLuowei = _characterCom.GetChild("Button_luowei").asLoader;
            _buttonYang = _characterCom.GetChild("Button_yang").asLoader;
            _buttonPolo = _characterCom.GetChild("Button_polo").asLoader;
            _buttonXiaoan = _characterCom.GetChild("Button_xiaoan").asLoader;
            _buttonLily = _characterCom.GetChild("Button_lily").asLoader;
            _buttonXuela = _characterCom.GetChild("Button_xuela").asLoader;

            _textLuowei = _characterCom.GetChild("Text_luowei").asTextField;
            _textYang = _characterCom.GetChild("Text_yang").asTextField;
            _textPolo = _characterCom.GetChild("Text_polo").asTextField;
            _textXiaoan = _characterCom.GetChild("Text_xiaoan").asTextField;
            _textLily = _characterCom.GetChild("Text_lily").asTextField;
            _textXuela = _characterCom.GetChild("Text_xuela").asTextField;

            _buttonLuowei.onClick.Add((() => { ChooseCharacter(Characters.LuoWei); }));
            _buttonYang.onClick.Add((() => { ChooseCharacter(Characters.Yang); }));
            _buttonPolo.onClick.Add(() => { ChooseCharacter(Characters.Polo); });
            _buttonXiaoan.onClick.Add(() => { ChooseCharacter(Characters.XiaoAn); });
            _buttonLily.onClick.Add((() => { ChooseCharacter(Characters.Lily); }));
            _buttonXuela.onClick.Add((() => { ChooseCharacter(Characters.Xuela); }));

            _buttonContinue = _characterCom.GetChild("Button_Continue").asButton;
            _buttonContinue.onClick.Add(() => { _characterCom.visible = false; });
        }

        public void Show()
        {
            _uiRoot.AddChild(_characterCom);
        }

        private static void ChooseCharacter(Characters character)
        {
            MyGameNetWorkManager.Instance.CallChooseCharacter(character);
            MyGameManager.Instance.localPlayerController.nowCharacter = character;
        }

        public void UpdateCharacterChooseState()
        {
            _textLily.SetVar("account", "").FlushVars();
            _textPolo.SetVar("account", "").FlushVars();
            _textXuela.SetVar("account", "").FlushVars();
            _textYang.SetVar("account", "").FlushVars();
            _textLuowei.SetVar("account", "").FlushVars();
            _textXiaoan.SetVar("account", "").FlushVars();

            foreach (var characterChooseListNode in MyGameNetWorkManager.Instance.NetLobbyPlayersCharacterStates)
            {
                var textToSet = $"({characterChooseListNode.AccountName.ToString()})";
                foreach (var playerController in MyGameManager.Instance.allPlayers.Where(playerController =>
                             playerController.playerAccountName == characterChooseListNode.AccountName))
                {
                    playerController.nowCharacter = characterChooseListNode.CharacterToChoose;
                }

                switch (characterChooseListNode.CharacterToChoose)
                {
                    case Characters.Lily:
                        _textLily.SetVar("account", textToSet).FlushVars();
                        break;
                    case Characters.Polo:
                        _textPolo.SetVar("account", textToSet).FlushVars();
                        break;
                    case Characters.Xuela:
                        _textXuela.SetVar("account", textToSet).FlushVars();
                        break;
                    case Characters.Yang:
                        _textYang.SetVar("account", textToSet).FlushVars();
                        break;
                    case Characters.LuoWei:
                        _textLuowei.SetVar("account", textToSet).FlushVars();
                        break;
                    case Characters.XiaoAn:
                        _textXiaoan.SetVar("account", textToSet).FlushVars();
                        break;
                    case Characters.None:
                    default:
                        break;
                }
            }

            _buttonContinue.visible =
                MyGameManager.Instance.allPlayers.All(playerController =>
                    playerController.nowCharacter != Characters.None) &&
                MyGameNetWorkManager.Instance.NetLobbyPlayersCharacterStates.Count ==
                MyGameManager.Instance.allPlayers.Count;
        }
    }
}