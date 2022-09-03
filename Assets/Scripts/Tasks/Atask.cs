using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using Player;
using UI.Game;
using Unity.Netcode;
using UnityEngine;

namespace Tasks
{
    public class Atask : TaskUtil
    {
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            if (other.CompareTag("Player") && other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
               
            }
        }

        protected override void OnTriggerExit(Collider other)
        {
            base.OnTriggerExit(other);
            if (other.CompareTag("Player") && other.gameObject.GetComponent<NetworkObject>().IsLocalPlayer)
            {
                
            }
        }
    }
}