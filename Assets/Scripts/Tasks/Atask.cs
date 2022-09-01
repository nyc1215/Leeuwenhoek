using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Tasks
{
    public class Atask : TaskUtil
    {
        private readonly NetworkVariable<Color> _netSpriteColor = new();

        private SpriteRenderer _spriteRenderer;

        protected override void Awake()
        {
            base.Awake();

            _spriteRenderer = GetComponent<SpriteRenderer>();
            _netSpriteColor.OnValueChanged = (value, newValue) => { _spriteRenderer.color = newValue; };
        }

        protected override void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (IsServer || IsHost)
                {
                    CommitSpriteColorClientRpc(Color.yellow);
                }
                else
                {
                    CommitSpriteColorServerRpc(Color.yellow);
                }
            }
        }

        protected override void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (IsServer || IsHost)
                {
                    CommitSpriteColorClientRpc(Color.white);
                }
                else
                {
                    CommitSpriteColorServerRpc(Color.white);
                }
            }
        }

        [ServerRpc]
        private void CommitSpriteColorServerRpc(Color color)
        {
            _netSpriteColor.Value = color;
        }

        [ClientRpc]
        private void CommitSpriteColorClientRpc(Color color)
        {
            _netSpriteColor.Value = color;
        }
    }
}