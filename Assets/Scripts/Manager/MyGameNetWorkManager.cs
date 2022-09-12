using System;
using System.Collections.Generic;
using FairyGUI;
using Player;
using UI.Room;
using UI.Util;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Manager
{
    public enum Characters
    {
        LuoWei,
        Yang,
        Polo,
        XiaoAn,
        Lily,
        Xuela,
        None
    }

    public class MyGameNetWorkManager : NetworkBehaviour
    {
        [Header("游戏总进度")] [Range(0, 100)] private readonly NetworkVariable<int> _gameTotalProgress = new();
        public readonly NetworkVariable<bool> GameIsEnd = new();

        public NetworkList<LobbyPlayerCharacterState> NetLobbyPlayersCharacterStates;

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

            NetLobbyPlayersCharacterStates = new NetworkList<LobbyPlayerCharacterState>();
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

            NetLobbyPlayersCharacterStates.OnListChanged += OnLobbyPlayerStateChanged;
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

        public void CallChooseCharacter(Characters character)
        {
            ChooseCharacterServerRpc(MyGameManager.Instance.LocalPlayerInfo.AccountName, character);
        }

        [ServerRpc(RequireOwnership = false)]
        private void ChooseCharacterServerRpc(FixedString32Bytes accountName, Characters character)
        {
            var listIndex = -1;
            for (var i = 0; i < NetLobbyPlayersCharacterStates.Count; i++)
            {
                if (NetLobbyPlayersCharacterStates[i].AccountName != accountName)
                {
                    continue;
                }

                listIndex = i;
                break;
            }


            if (listIndex == -1)
            {
                NetLobbyPlayersCharacterStates.Add(CheckChooseCharacter(accountName, character)
                    ? new LobbyPlayerCharacterState(accountName, character)
                    : new LobbyPlayerCharacterState(accountName, Characters.None));
            }
            else
            {
                NetLobbyPlayersCharacterStates[listIndex] = new LobbyPlayerCharacterState(accountName,
                    CheckChooseCharacter(accountName, character) ? character : Characters.None);
            }
        }

        private bool CheckChooseCharacter(FixedString32Bytes accountName, Characters character)
        {
            for (var i = 0; i < NetLobbyPlayersCharacterStates.Count; i++)
            {
                if (NetLobbyPlayersCharacterStates[i].CharacterToChoose == character)
                {
                    if (NetLobbyPlayersCharacterStates[i].AccountName != accountName)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void OnLobbyPlayerStateChanged(NetworkListEvent<LobbyPlayerCharacterState> changeEvent)
        {
            FindObjectOfType<RoomUIPanel>().CharacterPanel?.UpdateCharacterChooseState();
        }

        #endregion

        private void EndGame()
        {
            StopAllCoroutines();
            MyGameManager.Instance.localPlayerController.OnDisable();
            UIOperationUtil.GoToScene(MyGameManager.Instance.uiJumpData.endMenu);
        }
    }

    public struct LobbyPlayerCharacterState : INetworkSerializable, IEquatable<LobbyPlayerCharacterState>
    {
        public FixedString32Bytes AccountName;

        public Characters CharacterToChoose;


        public LobbyPlayerCharacterState(FixedString32Bytes accountName, Characters characterToChoose)
        {
            AccountName = accountName;
            CharacterToChoose = characterToChoose;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AccountName);
            serializer.SerializeValue(ref CharacterToChoose);
        }

        public bool Equals(LobbyPlayerCharacterState other)
        {
            return AccountName == other.AccountName && CharacterToChoose.Equals(other.CharacterToChoose);
        }
    }
}