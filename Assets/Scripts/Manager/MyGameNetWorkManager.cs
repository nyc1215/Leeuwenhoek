using System;
using FairyGUI;
using Unity.Netcode;
using UnityEngine;

namespace Manager
{
    public class MyGameNetWorkManager : NetworkBehaviour
    {
        [Header("游戏总进度")] [Range(0, 100)] private readonly NetworkVariable<int> _gameTotalProgress = new();

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
            _gameTotalProgress.Value = 0;
            _gameTotalProgress.OnValueChanged += (value, newValue) =>
            {
                if (GameProgressBar != null)
                {
                    GameProgressBar.value = newValue;
                }
                if (newValue >= 100)
                {
                    MyGameManager.Instance.GameIsEnd = true;
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

        public void AddGameProgress(int addProgress)
        {
            CommitGameProgressServerRpc(_gameTotalProgress.Value + addProgress);
        }

        #endregion
    }
}