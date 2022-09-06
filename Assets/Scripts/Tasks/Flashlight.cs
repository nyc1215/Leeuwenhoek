using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace Tasks
{
    public class Flashlight : TaskUtil
    {
        private readonly List<GLoader> _batteryList = new();
        private GImage _flashlight;
        private GTextField _successText;
        private GGraph _successTextBg;

        private bool _isDragging;
        private int _successNum;

        protected override void Awake()
        {
            TaskPanelName = "Flashlight";

            base.Awake();

            _batteryList.Add(TaskUI.GetChild("battery1").asLoader);
            _batteryList.Add(TaskUI.GetChild("battery2").asLoader);
            _batteryList.Add(TaskUI.GetChild("battery3").asLoader);
            foreach (var battery in _batteryList)
            {
                battery.pivot = new Vector2(0.5f, 0.5f);
                battery.pivotAsAnchor = true;
                battery.draggable = true;
                battery.dragBounds = new Rect(560, 240, 800, 600);
                battery.onDragStart.Add(() =>
                {
                    if (!_isDragging)
                    {
                        _isDragging = true;
                    }
                    else
                    {
                        battery.StopDrag();
                    }
                });
                battery.onDragEnd.Add(() =>
                {
                    BatteryDragEnd(battery);
                    _isDragging = false;
                });
            }


            _flashlight = TaskUI.GetChild("Img_Flashlight").asImage;
            _flashlight.pivot = new Vector2(0.5f, 0.5f);
            _flashlight.pivotAsAnchor = true;

            _successText = TaskUI.GetChild("Success_Text").asTextField;
            _successTextBg = TaskUI.GetChild("Success_TextBG").asGraph;
        }

        protected override void InitTask()
        {
            _successNum = 0;
            _isDragging = false;
            _successText.visible = false;
            _successTextBg.visible = false;
        }

        private void BatteryDragEnd(GObject image)
        {
            var distanceXY = image.xy - _flashlight.xy;
            if (distanceXY.x < image.width / 2f + _flashlight.width / 2f && distanceXY.y < image.height / 2f + _flashlight.height / 2f)
            {
                image.draggable = false;
                image.visible = false;
                _successNum++;
            }

            if (_successNum == _batteryList.Count)
            {
                IsSuccess = true;
                ShowSuccessText();
            }
        }

        private void ShowSuccessText()
        {
            _successText.visible = true;
            _successTextBg.visible = true;

            foreach (var battery in _batteryList)
            {
                battery.draggable = false;
            }
        }
    }
}