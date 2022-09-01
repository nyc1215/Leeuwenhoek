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
            _gameTotalProgress.OnValueChanged += (value, newValue) =>
            {
                if (GameProgressBar != null)
                {
                    if (IsServer)
                    {
                        GameProgressBar.value = newValue;
                        SyncGameProgressClientRpc(newValue);
                    }
                }
            };
        }

        [ServerRpc(RequireOwnership = false)]
        private void CommitGameProgressServerRpc(int progress)
        {
            if (!IsClient)
            {
                _gameTotalProgress.Value = progress;
            }
        }

        [ClientRpc]
        private void SyncGameProgressClientRpc(int progress)
        {
            GameProgressBar.value = progress;
        }

        public void AddGameProgress(int addProgress)
        {
            // if (IsOwner)
            // {
            //     CommitGameProgressServerRpc(_gameTotalProgress.Value += addProgress);
            // }
            CommitGameProgressServerRpc(_gameTotalProgress.Value += addProgress);
        }

        #endregion
    }
}