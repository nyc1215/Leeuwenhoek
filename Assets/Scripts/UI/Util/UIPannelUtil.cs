using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FairyGUI;

namespace UI.Util
{
    public class UIPannelUtil : MonoBehaviour
    {
        public List<string> ButtonNames = new();
        protected Dictionary<string, GButton> ButtonList = new();
    }
}