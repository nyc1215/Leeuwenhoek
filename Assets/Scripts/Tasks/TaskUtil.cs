using System;
using FairyGUI;
using Manager;
using Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Tasks
{
    public class TaskUtil : MonoBehaviour
    {
        public bool isDoing;
        public int addProgress = 5;
        private bool _isSuccess;
        private Window _taskWindow;
        private GComponent taskUI;
        private GButton taskQuitButton;

        protected virtual void Awake()
        {
            UIPackage.AddPackage("FairyGUIOutPut/Game");
            taskUI = UIPackage.CreateObject("Game", "TaskPanel").asCom;
            taskQuitButton = taskUI.GetChild("Button_Back").asButton;

            _taskWindow = new Window
            {
                contentPane = taskUI,
                modal = true,
                closeButton = taskQuitButton,
                gameObjectName = "UIPanel"
            };
            _taskWindow.closeButton.onClick.Add(EndTask);
            isDoing = false;
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                MyGameManager.Instance.localPlayerController.nowTask = this;
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                MyGameManager.Instance.localPlayerController.nowTask = null;
            }
        }


        public void StartTask()
        {
            if (!isDoing)
            {
                isDoing = true;
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
        }

        private void EndTask()
        {
            _isSuccess = true;
            if (_isSuccess)
            {
                MyGameNetWorkManager.Instance.AddGameProgress(addProgress);
            }
            else
            {
                InitTask();
            }

            isDoing = false;
        }
    }
}