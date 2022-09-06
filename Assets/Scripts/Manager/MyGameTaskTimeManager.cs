using System.Collections.Generic;
using Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Manager
{
    public class MyGameTaskTimeManager : NetworkBehaviour
    {
        public static MyGameNetWorkManager Instance { get; private set; }

        public List<TaskUtil> AllTasks = new ();

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
        }

        #endregion
    }
}