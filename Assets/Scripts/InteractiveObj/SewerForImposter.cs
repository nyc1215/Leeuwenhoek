using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

namespace InteractiveObj
{
    /// <summary>
    /// 狼人传送下水道
    /// </summary>
    public class SewerForImposter : MonoBehaviour
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
                if (other.gameObject.GetComponent<MyPlayerController>().isImposter)
                {
                    _spriteRenderer.color = Color.yellow;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (other.gameObject.GetComponent<MyPlayerController>().isImposter)
                {
                    _spriteRenderer.color = Color.white;
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