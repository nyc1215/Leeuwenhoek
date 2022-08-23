using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using UI.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Game
{
    public class GameUIPanel : UIPanelUtil
    {
        public GameObject canvasObject;

        private GoWrapper _gw;

        protected override void Awake()
        {
            base.Awake();
            _gw = new GoWrapper(canvasObject);
        }
    }
}