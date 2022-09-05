using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace UI.Util
{
    /// <summary>
    /// UI面板基础类
    /// </summary>
    public class UIPanelUtil : MonoBehaviour
    {
        public List<string> buttonNames = new();
        private readonly Dictionary<string, GButton> _buttonList = new();
        protected GComponent UIRoot;

        protected virtual void Awake()
        {
            UIPackage.AddPackage("FairyGUIOutPut/Public");

            UIRoot = GetComponent<UIPanel>().ui;

            Assert.IsNotNull(UIRoot);

            GetAllButtons();
        }

        private void GetAllButtons()
        {
            if (buttonNames.Count == 0)
            {
                Debug.LogWarning($"{GetType().Name}'s buttonNames is empty!!!");
                return;
            }

            foreach (var btnName in buttonNames)
            {
                var aButton = UIRoot?.GetChild(btnName)?.asButton;
                if (aButton == null)
                {
                    continue;
                }
                _buttonList.Add(btnName, aButton);
            }
        }

        protected GButton GetButton(string buttonName)
        {
            if (_buttonList.Count == 0)
            {
                Debug.LogWarning($"{GetType().Name}'s buttonList is empty!!!");
                return null;
            }

            if (!_buttonList.ContainsKey(buttonName))
            {
                Debug.LogWarning($"{buttonName} is not found in panel");
                return null;
            }

            return _buttonList[buttonName];
        }
    }
}