using System;
using FairyGUI;
using UI.Util;
using Unity.Netcode;
using UnityEngine;

namespace Manager
{
    public class MyGameNetWorkManager : NetworkBehaviour
    {
        [Header("游戏总进度")] [Range(0, 100)] private readonly NetworkVariable<int> _gameTotalProgress = new();
        public readonly NetworkVariable<bool> GameIsEnd = new();


        public GProgressBar GameProgressBar;

        public static MyGameNetWorkManager Instance { get; private set; }

        #region MonoBehavior

        private void Awake()
        {
            if (Instance != null)
            {
                return;
            }

            Instance = FindObjectOfType(typeof(MyGameNetWorkManager)) as MyGameNetWorkManager;
            if (Application.isPlaying)
            {
                transform.SetParent(null);
                DontDestroyOnLoad(gameObject);
            }
        }

        #endregion

        #region NetCode

        public override void OnNetworkSpawn()
        {
            GameIsEnd.Value = false;
            GameIsEnd.OnValueChanged += (value, newValue) =>
            {
                if (newValue)
                {
                    EndGame();
                }
            }; 
            _gameTotalProgress.Value = 0;
            _gameTotalProgress.OnValueChanged += (value, newValue) =>
            {
                if (GameProgressBar != null)
                {
                    GameProgressBar.value = newValue;
                }

                if (newValue >= 100)
                {
                    CommitGameEndServerRpc(true);
                }
            };
        }

        [ServerRpc(RequireOwnership = false)]
        private void CommitGameProgressServerRpc(int progress)
        {
            if (IsServer || IsHost)
            {
                _gameTotalProgress.Value = progress;
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void CommitGameEndServerRpc(bool isEnd)
        {
            if (IsServer || IsHost)
            {
                GameIsEnd.Value = isEnd;
            }
        }

        public void AddGameProgress(int addProgress)
        {
            CommitGameProgressServerRpc(_gameTotalProgress.Value + addProgress);
        }

        #endregion

        private void EndGame()
        {
            StopAllCoroutines();
            MyGameManager.Instance.localPlayerController.OnDisable();
            UIOperationUtil.GoToScene(MyGameManager.Instance.uiJumpData.endMenu);
        }
    }
}