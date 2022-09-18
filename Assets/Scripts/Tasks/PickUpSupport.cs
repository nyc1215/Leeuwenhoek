using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace Tasks
{
    public class PickUpSupport : TaskUtil
    {
        private readonly List<GLoader> _supportList = new();
        private GImage _bag;
        private GTextField _successText;
        private GGraph _successTextBg;

        private bool _isDragging;
        private int _successNum;

        protected override void Awake()
        {
            TaskPanelName = "PickUpSupport";

            base.Awake();
            _supportList.Clear();
            _supportList.Add(TaskUI.GetChild("support1").asLoader);
            _supportList.Add(TaskUI.GetChild("support2").asLoader);
            _supportList.Add(TaskUI.GetChild("support3").asLoader);
            foreach (var support in _supportList)
            {
                support.pivot = new Vector2(0.5f, 0.5f);
                support.pivotAsAnchor = true;
                support.draggable = true;
                support.dragBounds = new Rect(560, 240, 800, 600);
                support.onDragStart.Add(() =>
                {
                    if (!_isDragging)
                    {
                        _isDragging = true;
                    }
                    else
                    {
                        support.StopDrag();
                    }
                });
                support.onDragEnd.Add(() =>
                {
                    BatteryDragEnd(support);
                    _isDragging = false;
                });
            }


            _bag = TaskUI.GetChild("Img_Bag").asImage;
            _bag.pivot = new Vector2(0.5f, 0.5f);
            _bag.pivotAsAnchor = true;

            _successText = TaskUI.GetChild("Success_Text").asTextField;
            _successTextBg = TaskUI.GetChild("Success_TextBG").asGraph;
        }

        protected override void InitTask()
        {
            base.InitTask();
            _successNum = 0;
            _isDragging = false;
            _successText.visible = false;
            _successTextBg.visible = false;
        }

        private void BatteryDragEnd(GObject image)
        {
            var distanceXY = image.xy - _bag.xy;
            if (distanceXY.x < image.width / 2f + _bag.width / 2f && distanceXY.y < image.height / 2f + _bag.height / 2f)
            {
                image.draggable = false;
                image.visible = false;
                _successNum++;
            }

            if (_successNum == _supportList.Count)
            {
                isSuccess = true;
                ShowSuccessText();
            }
        }

        private void ShowSuccessText()
        {
            _successText.visible = true;
            _successTextBg.visible = true;

            foreach (var support in _supportList)
            {
                support.draggable = false;
            }
        }
    }
}