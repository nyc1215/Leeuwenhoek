using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    /// <summary>
    /// 单例模板类
    /// 参照Mirror.NetworkManager的单例设计
    /// <see cref="Mirror.NetworkManager"/>
    /// </summary>
    public class SingleTon<T> : MonoBehaviour where T : MonoBehaviour
    {
        [Tooltip("切换场景时是否销毁该单例对象")] public bool dontDestroyOnLoad = true;
        public static T Instance { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null)
            {
                return;
            }

            if (dontDestroyOnLoad)
            {
                Instance = this as T;
                if (Application.isPlaying)
                {
                    transform.SetParent(null);
                    DontDestroyOnLoad(gameObject);
                }
            }
            else
            {
                Instance = this as T;
            }
        }
    }
}