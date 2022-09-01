using System;
using FairyGUI;
using Manager;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Tasks
{
    public abstract class TaskUtil : NetworkBehaviour
    {
        public InputAction interactiveKey;
        public bool isDoing;
        public int addProgress = 5;
        private bool _isSuccess;
        private Window _taskWindow;

        protected virtual void Awake()
        {
            UIPackage.AddPackage("FairyGUIOutPut/Game");
            var taskUI = UIPackage.CreateObject("Game", "TaskPanel").asCom;
            var taskQuitButton = taskUI.GetChild("Button_Back").asButton;

            _taskWindow = new Window
            {
                contentPane = taskUI,
                modal = true,
                closeButton = taskQuitButton
            };
            _taskWindow.Hide();
            _taskWindow.closeButton.onClick.Add(EndTask);
            isDoing = false;
        }

        private void OnEnable()
        {
            interactiveKey.performed += StartTask;
        }

        private void OnDisable()
        {
            interactiveKey.performed -= StartTask;
        }

        protected abstract void OnTriggerEnter(Collider other);

        protected abstract void OnTriggerExit(Collider other);


        private void StartTask(InputAction.CallbackContext context)
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