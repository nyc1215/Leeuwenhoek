using System;
using FairyGUI;
using Manager;
using UI.Room;
using UI.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.End
{
    public class EndUIPanel : UIPanelUtil
    {
        public RoomReadyStory roomReadyStory;

        private GButton _quitButton;

        private GComponent _storyCom;
        private GTextField _storyTextField;
        private GButton _closeStory;

        protected override void Awake()
        {
            base.Awake();

            _storyCom = UIRoot.AddChild(UIPackage.CreateObject("End", "EndStoryPanel")).asCom;
            _storyTextField = _storyCom.GetChild("Text_Letter").asTextField;
            _closeStory = _storyCom.GetChild("Button_Back").asButton;

            _quitButton = GetButton("Button_Back");
        }

        private void Start()
        {

            _quitButton.onClick.Add(() => { UIOperationUtil.GoToScene(MyGameManager.Instance.uiJumpData.bootMenu); });
            _closeStory.onClick.Add(() =>
            {
                _storyCom.Dispose();
                MyGameNetWorkManager.Instance.CommitDestroyAllPlayersServerRpc();
            });
            
            Debug.Log(MyGameManager.Instance.whoIsImposter.ToString());
            switch (MyGameManager.Instance.whoIsImposter)
            {
                case Characters.LuoWei:
                    _storyTextField.text = roomReadyStory.storyText[0];
                    break;
                case Characters.Yang:
                    _storyTextField.text = roomReadyStory.storyText[1];
                    break;
                case Characters.Polo:
                    _storyTextField.text = roomReadyStory.storyText[2];
                    break;
                case Characters.Lily:
                    _storyTextField.text = roomReadyStory.storyText[3];
                    break;
                case Characters.XiaoAn:
                    _storyTextField.text = roomReadyStory.storyText[5];
                    break;
                case Characters.Xuela:
                    _storyTextField.text = roomReadyStory.storyText[4];
                    break;
                case Characters.None:
                default:
                    break;
            }
        }
    }
}