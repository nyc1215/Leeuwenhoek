using System;
using FairyGUI;
using Manager;
using UnityEngine;

namespace UI.Game
{
    public class GameProgressEvent : MonoBehaviour
    {
        private GProgressBar _gameProgressBar;

        private void Awake()
        {
            _gameProgressBar = GetComponent<UIPanel>().ui.GetChild("ProgressBar_Game").asProgress;
            
        }
    }
}