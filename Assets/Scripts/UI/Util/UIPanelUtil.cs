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
        protected Dictionary<string, GButton> ButtonList = new();
        protected UIPanel Panel;
        protected GComponent UIRoot;

        protected virtual void Awake()
        {
            Panel = GetComponent<UIPanel>();
            UIRoot = Panel.ui;

            Assert.IsNotNull(Panel);
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
                var aButton = UIRoot?.GetChild(btnName).asButton;
                ButtonList.Add(btnName, aButton);
            }
        }

        protected GButton GetButton(string buttonName)
        {
            if (ButtonList.Count == 0)
            {
                Debug.LogWarning($"{GetType().Name}'s buttonList is empty!!!");
                return null;
            }

            if (!ButtonList.ContainsKey(buttonName))
            {
                Debug.LogWarning($"{buttonName} is not found in panel");
                return null;
            }

            return ButtonList[buttonName];
        }
    }
}