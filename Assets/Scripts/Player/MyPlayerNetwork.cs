using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Player
{
    public class MyPlayerNetwork : NetworkBehaviour
    {
        private readonly NetworkVariable<PlayerNetworkData> _netSpriteScale =
            new(writePerm: NetworkVariableWritePermission.Owner);

        private Transform _spriteTransform;

        private void Awake()
        {
            _spriteTransform = transform.GetChild(0);
        }

        private void Update()
        {
            if (IsOwner)
            {
                _netSpriteScale.Value = new PlayerNetworkData()
                {
                    spriteScale = _spriteTransform.localScale
                };
            }
            else
            {
                _spriteTransform.localScale = _netSpriteScale.Value.spriteScale;
            }
        }

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
    }
}