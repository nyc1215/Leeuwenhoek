﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using Player;
using UI.Game;
using UI.Room;
using UI.Util;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

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
        [Header("报告时间")] [Min(1)] public int time = 60;
        private Coroutine _reportTimeCoroutine;

        [Header("游戏总进度")] [Range(0, 100)] private readonly NetworkVariable<int> _gameTotalProgress = new();
        public readonly NetworkVariable<bool> GameIsEnd = new();

        public NetworkList<LobbyPlayerCharacterState> NetLobbyPlayersCharacterStates;

        public NetworkVariable<int> whoWillBeKicked = new();

        private readonly NetworkVariable<bool> _netIsAlreadyChooseImposter = new();
        public readonly NetworkVariable<int> NetGoodPlayerNum = new();
        public readonly NetworkVariable<bool> NetImposterIsWin = new();
        public readonly NetworkVariable<bool> NetGoodIsWin = new();
        private readonly NetworkVariable<FixedString32Bytes> _netImposterName = new();
        private NetworkList<ulong> _netBodyId;


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
            _netBodyId = new NetworkList<ulong>();
        }

        #endregion

        #region NetCode

        public override void OnNetworkSpawn()
        {
            GameIsEnd.Value = false;
            GameIsEnd.OnValueChanged += (_, newValue) =>
            {
                if (newValue)
                {
                    EndGame();
                }
            };
            _gameTotalProgress.Value = 0;
            _gameTotalProgress.OnValueChanged += (_, newValue) =>
            {
                if (GameProgressBar != null)
                {
                    GameProgressBar.value = newValue;
                    FindObjectOfType<GameUIPanel>().ShowStories();
                }

                if (newValue >= 100)
                {
                    CommitGoodPlayerWinServerRpc(true);
                    CommitGameEndServerRpc(true);
                }
            };

            whoWillBeKicked.Value = -1;
            NetLobbyPlayersCharacterStates.OnListChanged += OnLobbyPlayerStateChanged;
            _netIsAlreadyChooseImposter.Value = false;
            NetGoodPlayerNum.Value = 0;
            NetImposterIsWin.Value = false;
            NetGoodPlayerNum.OnValueChanged += (_, newValue) =>
            {
                if (MyGameManager.CompareScene(MyGameManager.Instance.uiJumpData.gameMenu))
                {
                    FindObjectOfType<GameUIPanel>().SetPlayerNumText(newValue + 1);
                    if (newValue <= 0)
                    {
                        CommitImposterWinServerRpc(true);
                        CommitGameEndServerRpc(true);
                    }
                }
            };
        }

        [ServerRpc(RequireOwnership = false)]
        public void RandomSetImposterServerRpc()
        {
            if (IsServer || IsHost)
            {
                NetGoodPlayerNum.Value = MyGameManager.Instance.PlayerListData.PlayerList.Count - 1;
                if (_netIsAlreadyChooseImposter.Value)
                {
                    return;
                }


                if (NetworkManager.Singleton.ConnectedClientsList.Count ==
                    MyGameManager.Instance.PlayerListData.PlayerList.Count)
                {
                    _netIsAlreadyChooseImposter.Value = true;

                    var imposterName = MyGameManager.Instance.PlayerListData
                        .PlayerList[Random.Range(0, MyGameManager.Instance.PlayerListData.PlayerList.Count)]
                        .AccountName;
                    SyncImposterClientRpc(imposterName);
                    _netImposterName.Value = imposterName;
                }
            }
        }

        [ClientRpc]
        private void SyncImposterClientRpc(FixedString32Bytes accountName)
        {
            foreach (var myPlayerController in MyGameManager.Instance.allPlayers)
            {
                var text = myPlayerController.gameObject.GetComponent<MyPlayerNetwork>().NetTopText.Value;
                myPlayerController.playerAccountName = text.ToString();
                if (text.Contains(accountName))
                {
                    myPlayerController.isImposter = true;
                }
            }

            Debug.Log($"accountName: {accountName} become imposter");
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
        public void CommitGoodPlayerNumServerRpc(int num)
        {
            if (IsServer || IsHost)
            {
                NetGoodPlayerNum.Value = num;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void CommitGameEndServerRpc(bool isEnd)
        {
            if (IsServer || IsHost)
            {
                GameIsEnd.Value = isEnd;
                NetLobbyPlayersCharacterStates.Clear();
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void CommitImposterWinServerRpc(bool isImposterWin)
        {
            if (IsServer || IsHost)
            {
                NetImposterIsWin.Value = isImposterWin;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void CommitGoodPlayerWinServerRpc(bool isWin)
        {
            if (IsServer || IsHost)
            {
                NetGoodIsWin.Value = isWin;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void CommitAddBodyIdServerRpc(ulong id)
        {
            _netBodyId.Add(id);
        }

        [ServerRpc(RequireOwnership = false)]
        public void CommitDesBodyIdServerRpc(ulong id)
        {
            if (_netBodyId.Contains(id))
            {
                _netBodyId.Remove(id);
            }
        }

        public override void OnDestroy()
        {
            NetLobbyPlayersCharacterStates?.Dispose();
            _netBodyId?.Dispose();
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
            if (MyGameManager.CompareScene(MyGameManager.Instance.uiJumpData.roomMenu))
            {
                FindObjectOfType<RoomUIPanel>().CharacterPanel?.UpdateCharacterChooseState();
            }

            if (MyGameManager.CompareScene(MyGameManager.Instance.uiJumpData.gameMenu))
            {
                var kickPanel = FindObjectOfType<GameUIPanel>().KickUIPanel;
                if (kickPanel == null)
                {
                    return;
                }

                if (kickPanel.KickUIPanelWindow.isShowing)
                {
                    kickPanel.UpdateVoteState();
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void InitVoteServerRpc()
        {
            var needToInit = false;
            foreach (var lobbyPlayersCharacterState in NetLobbyPlayersCharacterStates)
            {
                if (lobbyPlayersCharacterState.Vote == 0)
                {
                    continue;
                }

                needToInit = true;
                break;
            }

            if (!needToInit)
            {
                return;
            }

            for (var i = 0; i < NetLobbyPlayersCharacterStates.Count; i++)
            {
                var netLobbyPlayersCharacterState = NetLobbyPlayersCharacterStates[i];
                netLobbyPlayersCharacterState.Vote = 0;
                NetLobbyPlayersCharacterStates[i] = netLobbyPlayersCharacterState;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeVoteServerRpc(Characters character, bool isAdd)
        {
            for (var i = 0; i < NetLobbyPlayersCharacterStates.Count; i++)
            {
                if (NetLobbyPlayersCharacterStates[i].CharacterToChoose == character)
                {
                    var newNode = NetLobbyPlayersCharacterStates[i];
                    newNode.Vote = isAdd ? ++newNode.Vote : --newNode.Vote;
                    NetLobbyPlayersCharacterStates[i] = newNode;
                    CalculateWhoWillBekKickedServerRpc();
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        public void ChangeVoteTextServerRpc(Characters character, bool isTalking)
        {
            for (var i = 0; i < NetLobbyPlayersCharacterStates.Count; i++)
            {
                if (NetLobbyPlayersCharacterStates[i].CharacterToChoose == character)
                {
                    var newNode = NetLobbyPlayersCharacterStates[i];
                    newNode.IsTalking = isTalking;
                    NetLobbyPlayersCharacterStates[i] = newNode;
                }
            }
        }

        public void KickSomeOne()
        {
            if (whoWillBeKicked.Value == -1)
            {
                return;
            }

            Debug.Log($"kickSomeOne:{whoWillBeKicked.Value}");
            KickPlayerServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void KickPlayerServerRpc()
        {
            var newNode = NetLobbyPlayersCharacterStates[whoWillBeKicked.Value];
            newNode.IsKicked = true;
            NetLobbyPlayersCharacterStates[whoWillBeKicked.Value] = newNode;
            KickPlayerClientRpc(NetLobbyPlayersCharacterStates[whoWillBeKicked.Value].AccountName);
        }

        [ClientRpc]
        private void KickPlayerClientRpc(FixedString32Bytes playerName)
        {
            foreach (var playerController in MyGameManager.Instance.allPlayers.Where(
                         playerController =>
                             playerController.playerAccountName.Contains($"({playerName.ToString()})") &&
                             !playerController.isDead && !playerController.isKicked))
            {
                Debug.Log($"{playerController.playerAccountName} kicked");
                playerController.BeKick();
            }
        }

        [ServerRpc]
        private void CalculateWhoWillBekKickedServerRpc()
        {
            var voteList = new List<int>();
            for (var i = 0; i < NetLobbyPlayersCharacterStates.Count; i++)
            {
                voteList.Add(NetLobbyPlayersCharacterStates[i].Vote);
            }

            if (voteList.Count <= 1)
            {
                whoWillBeKicked.Value = -1;
                return;
            }

            var voteListSorted = new List<int>(voteList);
            voteListSorted.Sort((x, y) => -x.CompareTo(y));

            if (voteListSorted.Count <= 1)
            {
                whoWillBeKicked.Value = -1;
                return;
            }

            if (voteListSorted[0] == voteListSorted[1])
            {
                whoWillBeKicked.Value = -1;
                return;
            }

            whoWillBeKicked.Value = voteList.FindIndex(maxVote => maxVote == voteListSorted.First());

            var result = voteList.Aggregate("voteList:", (current, varInt) => current + (varInt + ","));
            Debug.Log(result);
            var result2 = voteListSorted.Aggregate("voteListSorted:", (current, varInt) => current + (varInt + ","));
            Debug.Log(result2);
            Debug.Log(whoWillBeKicked.Value.ToString());
            Debug.Log(NetLobbyPlayersCharacterStates[whoWillBeKicked.Value].AccountName);
        }

        #endregion

        private void EndGame()
        {
            CommitGameEndServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void CommitDestroyAllPlayersServerRpc()
        {
            if (IsServer || IsHost)
            {
                foreach (var playerController in MyGameManager.Instance.allPlayers.Where(playerController =>
                             playerController != null))
                {
                    playerController.OnDisable();
                    if (playerController.gameObject != null)
                    {
                        Destroy(playerController.gameObject);
                    }
                }
            }
        }

        [ServerRpc(RequireOwnership = false)]
        private void CommitGameEndServerRpc()
        {
            foreach (var id in _netBodyId)
            {
                if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.ContainsKey(id))
                {
                    NetworkManager.Singleton.SpawnManager.SpawnedObjects[id].Despawn();
                }
            }

            SyncGoToEndGameClientRpc();
        }


        [ClientRpc]
        private void SyncGoToEndGameClientRpc()
        {
            foreach (var lobbyPlayersCharacterState in NetLobbyPlayersCharacterStates)
            {
                if (lobbyPlayersCharacterState.AccountName == _netImposterName.Value)
                {
                    MyGameManager.Instance.whoIsImposter = lobbyPlayersCharacterState.CharacterToChoose;
                }
            }

            if (IsClient)
            {
                StopAllCoroutines();
                StartCoroutine(Wait(0.5f));
            }
        }

        public void StartReportCountDown()
        {
            if (_reportTimeCoroutine != null)
            {
                StopReportCountDown();
            }

            _reportTimeCoroutine = StartCoroutine(ReportCountDown());
        }

        public void StopReportCountDown()
        {
            if (_reportTimeCoroutine == null)
            {
                return;
            }

            StopCoroutine(_reportTimeCoroutine);
            _reportTimeCoroutine = null;
        }

        private IEnumerator ReportCountDown()
        {
            var allTime = time;
            var waitASec = new WaitForSeconds(1);
            var kickPanel = FindObjectOfType<GameUIPanel>().KickUIPanel;
            while (allTime >= 0)
            {
                kickPanel.UpdateCountDown(allTime);
                yield return waitASec;
                allTime--;
            }

            kickPanel.CountDownFinish();
        }

        private static IEnumerator Wait(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            UIOperationUtil.GoToScene(MyGameManager.Instance.uiJumpData.endMenu);
        }
    }

    public struct LobbyPlayerCharacterState : INetworkSerializable, IEquatable<LobbyPlayerCharacterState>
    {
        public FixedString32Bytes AccountName;

        public Characters CharacterToChoose;

        public int Vote;

        public bool IsDead;

        public bool IsTalking;

        public bool IsKicked;


        public LobbyPlayerCharacterState(FixedString32Bytes accountName, Characters characterToChoose, int vote = 0,
            bool isDead = false, bool isTalking = false, bool isKicked = false)
        {
            AccountName = accountName;
            CharacterToChoose = characterToChoose;
            Vote = vote;
            IsDead = isDead;
            IsTalking = isTalking;
            IsKicked = isKicked;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref AccountName);
            serializer.SerializeValue(ref CharacterToChoose);
            serializer.SerializeValue(ref Vote);
            serializer.SerializeValue(ref IsDead);
            serializer.SerializeValue(ref IsTalking);
            serializer.SerializeValue(ref IsKicked);
        }

        public bool Equals(LobbyPlayerCharacterState other)
        {
            return AccountName == other.AccountName && CharacterToChoose.Equals(other.CharacterToChoose);
        }
    }
}