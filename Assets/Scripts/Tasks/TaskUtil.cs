﻿using System;
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
    public class TaskUtil : MonoBehaviour
    {
        protected string TaskPanelName;
        public int addProgress = 5;

        public bool isSuccess;

        public Window TaskWindow;
        protected GComponent TaskUI;
        private GButton _taskQuitButton;
        protected SpriteRenderer _spriteRenderer;
        protected GameUIPanel _gameUIPanel;

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
                pivot = Vector2.zero,
                pivotAsAnchor = true,
                gameObjectName = "UIPanel"
            };
            TaskWindow.closeButton.onClick.Add(EndTask);
        }

        private void Start()
        {
            _gameUIPanel = FindObjectOfType<GameUIPanel>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") &&
                other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                if (other.gameObject.GetComponent<MyPlayerController>().isDead ||
                    other.gameObject.GetComponent<MyPlayerController>().isKicked)
                {
                    return;
                }

                if (isSuccess)
                {
                    return;
                }

                _gameUIPanel.ChangeTaskButtonVisible(true);
                MyGameManager.Instance.localPlayerController.nowTask = this;
            }
        }

        protected void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player") &&
                other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                if (other.gameObject.GetComponent<MyPlayerController>().isDead ||
                    other.gameObject.GetComponent<MyPlayerController>().isKicked)
                {
                    return;
                }

                if (isSuccess)
                {
                    _gameUIPanel.ChangeTaskButtonVisible(false);
                    MyGameManager.Instance.localPlayerController.nowTask = null;
                }
                else
                {
                    _gameUIPanel.ChangeTaskButtonVisible(true);
                    MyGameManager.Instance.localPlayerController.nowTask = this;
                }
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") &&
                other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                if (other.gameObject.GetComponent<MyPlayerController>().isDead ||
                    other.gameObject.GetComponent<MyPlayerController>().isKicked)
                {
                    return;
                }

                if (isSuccess)
                {
                    return;
                }

                _gameUIPanel.ChangeTaskButtonVisible(false);
                MyGameManager.Instance.localPlayerController.nowTask = null;
            }
        }


        public void StartTask()
        {
            if (isSuccess == false)
            {
                InitTask();
                OpenTaskUI();
            }
        }

        protected virtual void InitTask()
        {
            Awake();
        }

        protected virtual void OpenTaskUI()
        {
            TaskWindow.Show();
            TaskWindow.Center();
            MyGameManager.Instance.localPlayerController.OnDisable();
        }

        public virtual void EndTask()
        {
            if (isSuccess)
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

            MyGameManager.Instance.localPlayerController.OnEnable();
        }
    }
}