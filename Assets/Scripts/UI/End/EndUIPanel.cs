using System;
using System.Collections.Generic;
using FairyGUI;
using Manager;
using UI.Room;
using UI.Util;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.End
{
    public class EndUIPanel : UIPanelUtil
    {
        public EndStory endStory;

        private GButton _quitButton;

        private GComponent _storyCom;
        private GTextField _storyTextField;
        private GButton _closeStory;

        private int _endStoryIndex;
        private int _endStoryTotalNum;
        private List<string> _nowStoryList;

        protected override void Awake()
        {
            base.Awake();

            _storyCom = UIRoot.AddChild(UIPackage.CreateObject("End", "EndStoryPanel")).asCom;
            _storyTextField = _storyCom.GetChild("Text_Letter").asTextField;
            _closeStory = _storyCom.GetChild("Button_Back").asButton;

            _quitButton = GetButton("Button_Back");

            _endStoryIndex = 0;
            _endStoryTotalNum = 0;
        }

        private void Start()
        {
            _quitButton.onClick.Add(() =>
            {
                NetworkManager.Singleton.Shutdown();
                MyGameManager.Instance.allPlayers.Clear();
                UIOperationUtil.GoToScene(MyGameManager.Instance.uiJumpData.bootMenu);
            });
            _closeStory.onClick.Add(() =>
            {
                _endStoryIndex++;
                if (_endStoryIndex == _endStoryTotalNum)
                {
                    _storyCom.Dispose();
                    MyGameNetWorkManager.Instance.CommitDestroyAllPlayersServerRpc();
                    return;
                }

                _storyTextField.text = _nowStoryList[_endStoryIndex];
            });

            Debug.Log(MyGameManager.Instance.whoIsImposter.ToString());
            switch (MyGameManager.Instance.whoIsImposter)
            {
                case Characters.LuoWei:
                    _endStoryTotalNum = endStory.luoWeiEnd.Count;
                    _nowStoryList = endStory.luoWeiEnd;
                    break;
                case Characters.Yang:
                    _endStoryTotalNum = endStory.yangEnd.Count;
                    _nowStoryList = endStory.yangEnd;
                    break;
                case Characters.Polo:
                    _endStoryTotalNum = endStory.poloEnd.Count;
                    _nowStoryList = endStory.poloEnd;
                    break;
                case Characters.Lily:
                    _endStoryTotalNum = endStory.lilyEnd.Count;
                    _nowStoryList = endStory.lilyEnd;
                    break;
                case Characters.XiaoAn:
                    _endStoryTotalNum = endStory.xiaoAnEnd.Count;
                    _nowStoryList = endStory.xiaoAnEnd;
                    break;
                case Characters.Xuela:
                    _endStoryTotalNum = endStory.xueLaEnd.Count;
                    _nowStoryList = endStory.xueLaEnd;
                    break;
                case Characters.None:
                default:
                    break;
            }

            _storyTextField.text = _nowStoryList[_endStoryIndex];
        }
    }
}