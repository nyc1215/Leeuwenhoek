using Manager;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    public class MyPlayerNetwork : NetworkBehaviour
    {
        private readonly NetworkVariable<PlayerNetworkData> _netSpriteScale =
            new(writePerm: NetworkVariableWritePermission.Owner);

        private Transform _spriteTransform;

        [Tooltip("角色其他部分精灵渲染器")] public SpriteRenderer playerPartSpriteRenderer;
        private readonly NetworkVariable<Color> _netSpriteColor = new();

        [Tooltip("角色头顶文字")] public TextMeshPro playerTopText;
        private readonly NetworkVariable<Color> _netTopTextColor = new();
        private readonly NetworkVariable<FixedString64Bytes> _netTopText = new();

        [Tooltip("角色头顶语音图标")] public SpriteRenderer playerVoiceIcon;
        private readonly NetworkVariable<bool> _networkShowVoiceIcon = new();

        private readonly Color[] _playerRandomColors =
        {
            Color.cyan, Color.green, Color.white, Color.blue, Color.black, Color.grey, Color.red, Color.yellow,
        };

        #region Rpc

        [ServerRpc]
        private void CommitSpriteColorServerRpc(Color color)
        {
            _netSpriteColor.Value = color;
        }

        [ServerRpc]
        private void CommitTopTextColorServerRpc(Color color)
        {
            _netTopTextColor.Value = color;
        }

        [ServerRpc]
        private void CommitTopTextServerRpc(string text)
        {
            _netTopText.Value = text;
        }

        [ServerRpc]
        private void CommitVoiceIconServerRpc(bool isShow)
        {
            _networkShowVoiceIcon.Value = isShow;
        }

        #endregion


        #region NetworkBehaviour

        public override void OnNetworkSpawn()
        {
            if (IsLocalPlayer)
            {
                MyGameManager.Instance.localPlayerNetwork = this;
            }

            if (IsOwner)
            {
                CommitSpriteColorServerRpc(_playerRandomColors[Random.Range(0, _playerRandomColors.Length)]);
                CommitTopTextServerRpc(MyGameManager.Instance.LocalPlayerInfo.AccountName);
                ChangeTopTextColor(false);
                CommitVoiceIconServerRpc(false);
            }
            else
            {
                playerPartSpriteRenderer.color = _netSpriteColor.Value;
                playerTopText.text = _netTopText.Value.ToString();
                playerVoiceIcon.enabled = _networkShowVoiceIcon.Value;
            }
        }

        private void Awake()
        {
            _spriteTransform = transform.GetChild(0);
            _netSpriteColor.OnValueChanged += (value, newValue) => { playerPartSpriteRenderer.color = newValue; };
            _netTopTextColor.OnValueChanged += (value, newValue) => { playerTopText.color = newValue; };
            _netTopText.OnValueChanged += (value, newValue) => { playerTopText.text = newValue.ToString(); };
            _networkShowVoiceIcon.OnValueChanged += (value, newValue) => { playerVoiceIcon.enabled = newValue; };
        }

        public override void OnDestroy()
        {
            _netSpriteColor.OnValueChanged = null;
            _netTopTextColor.OnValueChanged = null;
            _netTopText.OnValueChanged = null;
            _networkShowVoiceIcon.OnValueChanged = null;
        }

        private void Update()
        {
            if (IsOwner)
            {
                _netSpriteScale.Value = new PlayerNetworkData
                {
                    spriteScale = _spriteTransform.localScale
                };
            }
            else
            {
                _spriteTransform.localScale = _netSpriteScale.Value.spriteScale;
            }
        }

        #endregion

        #region PlayerNetworkData

        private struct PlayerNetworkData : INetworkSerializable
        {
            private float _spriteScaleX;

            internal Vector2 spriteScale
            {
                get => new(_spriteScaleX, 1);
                set => _spriteScaleX = value.x;
            }

            public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
            {
                serializer.SerializeValue(ref _spriteScaleX);
            }
        }

        #endregion

        public void ChangeTopTextColor(bool isReady)
        {
            if (IsOwner)
            {
                CommitTopTextColorServerRpc(isReady ? Color.green : Color.white);
                CommitTopTextServerRpc(isReady
                    ? MyGameManager.Instance.LocalPlayerInfo.AccountName + "√"
                    : MyGameManager.Instance.LocalPlayerInfo.AccountName);
            }
        }

        public void ChangeVoiceIconShow(bool isShow)
        {
            if (IsOwner)
            {
                CommitVoiceIconServerRpc(isShow);
            }
        }
    }
}