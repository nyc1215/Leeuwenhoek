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
        public NetworkVariable<bool> isDoing = new();
        public int addProgress = 5;
        private bool _isSuccess;
        private Window _taskWindow;
        private GComponent taskUI;
        private GButton taskQuitButton;
        private SpriteRenderer _spriteRenderer;

        protected virtual void Awake()
        {
            UIPackage.AddPackage("FairyGUIOutPut/Game");
            taskUI = UIPackage.CreateObject("Game", "TaskPanel").asCom;
            taskQuitButton = taskUI.GetChild("Button_Back").asButton;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _taskWindow = new Window
            {
                contentPane = taskUI,
                modal = true,
                closeButton = taskQuitButton,
                gameObjectName = "UIPanel"
            };
            _taskWindow.closeButton.onClick.Add(EndTask);
        }

        public override void OnNetworkSpawn()
        {
            isDoing.Value = false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") &&
                other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                _spriteRenderer.color = Color.yellow;
                MyGameManager.Instance.localPlayerController.nowTask = this;
                FindObjectOfType<GameUIPanel>().ChangeTaskButtonVisible(true);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") &&
                other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                _spriteRenderer.color = Color.white;
                MyGameManager.Instance.localPlayerController.nowTask = null;
                FindObjectOfType<GameUIPanel>().ChangeTaskButtonVisible(false);
            }
        }


        public void StartTask()
        {
            if (isDoing.Value == false)
            {
                CommitDoingStateServerRpc(true);
                InitTask();
                StartTaskUI();
            }
        }

        private void InitTask()
        {
            _isSuccess = false;
        }

        private void StartTaskUI()
        {
            _taskWindow.Show();
            _taskWindow.Center();
            MyGameManager.Instance.localPlayerController.OnDisable();
        }

        private void EndTask()
        {
            if (_isSuccess)
            {
                MyGameNetWorkManager.Instance.AddGameProgress(addProgress);
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
            isDoing.Value = doing;
        }
    }
}