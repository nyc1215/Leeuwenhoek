using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Manager
{
    public class InitNetWork : MonoBehaviour
    {
        private void Awake()
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = MyGameManager.Instance.netOrLocal == NetOrLocal.Local ? "127.0.0.1" : "120.26.85.13";
        }
    }
}
