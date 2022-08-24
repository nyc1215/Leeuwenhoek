using System;
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
        private JoystickModule _joystick;

        private void Start()
        {
            _joystick = new JoystickModule(UIRoot);
        }
    }
}