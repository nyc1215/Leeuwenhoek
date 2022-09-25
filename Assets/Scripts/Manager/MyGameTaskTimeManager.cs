using System.Collections;
using System.Collections.Generic;
using Tasks;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Manager
{
    public class MyGameTaskTimeManager : NetworkBehaviour
    {
        public static MyGameNetWorkManager Instance { get; private set; }

        public List<TaskUtil> allTasks = new();

        public int changeTaskSecond;

        private Coroutine _changeTaskCoroutine;

        private WaitForSeconds _changeTaskWaitForSeconds;

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

        private void Start()
        {
            changeTaskSecond = 45;
            _changeTaskWaitForSeconds = new WaitForSeconds(changeTaskSecond);
            _changeTaskCoroutine ??= StartCoroutine(TaskRandomOpen());
        }

        private IEnumerator TaskRandomOpen()
        {
            while (!MyGameNetWorkManager.Instance.GameIsEnd.Value)
            {
                foreach (var taskUtil in allTasks)
                {
                    if (taskUtil.TaskWindow.isShowing)
                    {
                        taskUtil.EndTask();
                    }

                    taskUtil.gameObject.SetActive(false);
                }

                var openTaskNum = 0;
                while (openTaskNum <= 5)
                {
                    var task = allTasks[Random.Range(0, allTasks.Count)];
                    if (task.gameObject.activeInHierarchy)
                    {
                        continue;
                    }

                    task.gameObject.SetActive(true);
                    task.isSuccess = false;
                    openTaskNum++;
                }

                yield return _changeTaskWaitForSeconds;
            }
        }

        #endregion
    }
}