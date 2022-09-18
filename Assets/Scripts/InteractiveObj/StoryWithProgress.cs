using FairyGUI;
using Manager;
using Player;
using UI.Game;
using Unity.Netcode;
using UnityEngine;

namespace InteractiveObj
{
    public sealed class StoryWithProgress : MonoBehaviour
    {
        public string storyText;
        
        private Window _storyWindow;
        private GComponent _taskUI;
        private GButton _taskQuitButton;
        private GTextField _storyText;
        private SpriteRenderer _spriteRenderer;
        private GameUIPanel _gameUIPanel;

        private void Awake()
        {
            UIPackage.AddPackage("FairyGUIOutPut/Game");
            _taskUI = UIPackage.CreateObject("Game", "Story").asCom;
            
            _taskQuitButton = _taskUI.GetChild("Button_Back").asButton;
            _storyText = _taskUI.GetChild("Text_Story").asTextField;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _storyWindow = new Window
            {
                contentPane = _taskUI,
                modal = true,
                closeButton = _taskQuitButton,
                pivot = Vector2.zero,
                pivotAsAnchor = true,
                gameObjectName = "UIPanel"
            };
            _storyWindow.closeButton.onClick.Add(EndTask);
        }

        private void Start()
        {
            _storyText.text = storyText;
            _gameUIPanel = FindObjectOfType<GameUIPanel>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") &&
                other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                if (other.gameObject.GetComponent<MyPlayerController>().isDead ||
                    other.gameObject.GetComponent<MyPlayerController>().isKicked)
                {
                    return;
                }

                MyGameManager.Instance.localPlayerController.nowStory = this;
                _gameUIPanel.ChangeStoryButtonVisible(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") &&
                other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                if (other.gameObject.GetComponent<MyPlayerController>().isDead ||
                    other.gameObject.GetComponent<MyPlayerController>().isKicked)
                {
                    return;
                }

                MyGameManager.Instance.localPlayerController.nowTask = null;
                _gameUIPanel.ChangeStoryButtonVisible(false);
            }
        }

        public void OpenStoryUI()
        {
            _storyWindow.Show();
            _storyWindow.Center();
            MyGameManager.Instance.localPlayerController.OnDisable();
        }

        private void EndTask()
        {
            _spriteRenderer.color = Color.white;
            MyGameManager.Instance.localPlayerController.nowTask = null;
            _gameUIPanel.ChangeTaskButtonVisible(false);
            MyGameManager.Instance.localPlayerController.OnEnable();
        }
    }
}