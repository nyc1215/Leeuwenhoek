using System;
using FairyGUI;
using Manager;
using Player;
using UI.Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Tasks
{
    public class TaskUtil : NetworkBehaviour
    {
        protected string TaskPanelName;
        public int addProgress = 5;

        protected bool IsSuccess;
        private readonly NetworkVariable<bool> _isDoing = new();

        protected Window TaskWindow;
        protected GComponent TaskUI;
        private GButton _taskQuitButton;
        private SpriteRenderer _spriteRenderer;
        private GameUIPanel _gameUIPanel;

        protected virtual void Awake()
        {
            if (string.IsNullOrEmpty(TaskPanelName))
            {
                Debug.LogWarning($"{gameObject.name} -> TaskPanelName is null or empty");
            }
            
            UIPackage.AddPackage("FairyGUIOutPut/Game");
            TaskUI = string.IsNullOrEmpty(TaskPanelName)
                ? UIPackage.CreateObject("Game", "TaskPanel").asCom
                : UIPackage.CreateObject("Game", TaskPanelName).asCom;

            _taskQuitButton = TaskUI.GetChild("Button_Back").asButton;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            TaskWindow = new Window
            {
                contentPane = TaskUI,
                modal = true,
                closeButton = _taskQuitButton,
                gameObjectName = "UIPanel"
            };
            TaskWindow.closeButton.onClick.Add(EndTask);
        }

        private void Start()
        {
            _gameUIPanel = FindObjectOfType<GameUIPanel>();
        }

        public override void OnNetworkSpawn()
        {
            _isDoing.Value = false;
            IsSuccess = false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") &&
                other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                if (IsSuccess)
                {
                    return;
                }

                _spriteRenderer.color = Color.yellow;
                MyGameManager.Instance.localPlayerController.nowTask = this;
                _gameUIPanel.ChangeTaskButtonVisible(true);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") &&
                other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                if (IsSuccess)
                {
                    return;
                }

                _spriteRenderer.color = Color.white;
                MyGameManager.Instance.localPlayerController.nowTask = null;
                _gameUIPanel.ChangeTaskButtonVisible(false);
            }
        }


        public void StartTask()
        {
            if (_isDoing.Value == false && IsSuccess == false)
            {
                CommitDoingStateServerRpc(true);
                InitTask();
                OpenTaskUI();
            }
        }

        protected virtual void InitTask()
        {
        }

        protected virtual void OpenTaskUI()
        {
            TaskWindow.Show();
            TaskWindow.Center();
            MyGameManager.Instance.localPlayerController.OnDisable();
        }

        private void EndTask()
        {
            if (IsSuccess)
            {
                MyGameNetWorkManager.Instance.AddGameProgress(addProgress);
                TaskWindow.Hide();
                _spriteRenderer.color = Color.white;
                MyGameManager.Instance.localPlayerController.nowTask = null;
                _gameUIPanel.ChangeTaskButtonVisible(false);
            }
            else
            {
                InitTask();
            }

            CommitDoingStateServerRpc(false);
            MyGameManager.Instance.localPlayerController.OnEnable();
        }

        [ServerRpc]
        private void CommitDoingStateServerRpc(bool doing)
        {
            _isDoing.Value = doing;
        }
    }
}