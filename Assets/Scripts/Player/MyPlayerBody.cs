using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using TMPro;
using UI.Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Player
{
    public class MyPlayerBody : NetworkBehaviour
    {
        public SpriteRenderer bodySprite;
        public TextMeshPro textMeshPro;

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            MyGameNetWorkManager.Instance.CommitAddBodyIdServerRpc(NetworkObject.NetworkObjectId);
        }

        public void SetText(string text)
        {
            textMeshPro.text = text;
        }

        public void SetColor(Color color)
        {
            bodySprite.color = color;
        }

        private void OnEnable()
        {
            if (MyPlayerController.AllBodies != null)
            {
                MyPlayerController.AllBodies.Add(transform);
            }
        }

        public void Report()
        {
            Debug.Log("Reported");
            if (MyPlayerController.AllBodies != null)
            {
                MyPlayerController.AllBodies.Remove(transform);
            }
            MyGameManager.Instance.localPlayerController.DestroyBodyServerRpc(NetworkObject.NetworkObjectId);
        }

        public void SetBodyHide(bool isHide)
        {
            bodySprite.enabled = !isHide;
            textMeshPro.enabled = !isHide;
        }
    }
}