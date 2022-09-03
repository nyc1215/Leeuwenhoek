using System;
using Manager;
using Player;
using UI.Game;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace InteractiveObj
{
    /// <summary>
    /// 狼人传送下水道
    /// </summary>
    public class SewerForImposter : NetworkBehaviour
    {
        public SewerForImposter goToSewer;

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.gameObject.GetComponent<MyPlayerController>().isImposter &&
                    other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    _spriteRenderer.color = Color.yellow;
                    MyGameManager.Instance.localPlayerController.nowSewer = this;
                    FindObjectOfType<GameUIPanel>().ChangeSewerButtonVisible(true);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.gameObject.GetComponent<MyPlayerController>().isImposter &&
                    other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
                {
                    _spriteRenderer.color = Color.white;
                    MyGameManager.Instance.localPlayerController.nowSewer = null;
                    FindObjectOfType<GameUIPanel>().ChangeSewerButtonVisible(false);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (goToSewer != null)
            {
                Gizmos.DrawLine(transform.position, goToSewer.transform.position);
            }
        }

        public void GoOtherSewer(MyPlayerController playerController)
        {
            if (playerController.isImposter)
            {
                if (goToSewer != null)
                {
                    playerController.transform.position = goToSewer.transform.position;
                }
            }
        }
    }
}