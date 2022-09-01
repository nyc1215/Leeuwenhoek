using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Player;
using Unity.Netcode;
using UnityEngine;

namespace Tasks
{
    public class Atask : TaskUtil
    {
        private SpriteRenderer _spriteRenderer;

        protected override void Awake()
        {
            base.Awake();

            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (other.CompareTag("Player"))
            {
                _spriteRenderer.color = Color.yellow;
            }
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            if (other.CompareTag("Player"))
            {
                _spriteRenderer.color = Color.white;
            }
        }
    }
}