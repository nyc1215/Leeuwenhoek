using System.Collections.Generic;
using FairyGUI;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tasks
{
    public class TelescopicSight : TaskUtil
    {
        public int targetRange = 50;

        private readonly List<GLoader> _crossList = new();
        private GImage _target;
        private GTextField _successText;
        private GGraph _successTextBg;

        private bool _isDragging;
        private int _successNum;

        protected override void Awake()
        {
            TaskPanelName = "TelescopicSight";

            base.Awake();
            _crossList.Clear();
            _crossList.Add(TaskUI.GetChild("cross1").asLoader);
            _crossList.Add(TaskUI.GetChild("cross2").asLoader);
            _crossList.Add(TaskUI.GetChild("cross3").asLoader);
            foreach (var cross in _crossList)
            {
                cross.pivot = new Vector2(0.5f, 0.5f);
                cross.pivotAsAnchor = true;
                cross.draggable = true;
                cross.dragBounds = new Rect(560, 240, 800, 600);
                cross.onDragStart.Add(() =>
                {
                    if (!_isDragging)
                    {
                        _isDragging = true;
                    }
                    else
                    {
                        cross.StopDrag();
                    }
                });
                cross.onDragEnd.Add(() =>
                {
                    CrossDragEnd(cross);
                    _isDragging = false;
                });
            }


            _target = TaskUI.GetChild("Img_Target").asImage;
            _target.pivot = new Vector2(0.5f, 0.5f);
            _target.pivotAsAnchor = true;

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

        private void CrossDragEnd(GObject image)
        {
            var distance = (image.xy - _target.xy).magnitude;
            if (distance <= targetRange)
            {
                image.draggable = false;
                image.visible = false;
                _successNum++;
            }

            if (_successNum == _crossList.Count)
            {
                isSuccess = true;
                ShowSuccessText();
            }
        }

        private void ShowSuccessText()
        {
            _successText.visible = true;
            _successTextBg.visible = true;

            foreach (var cross in _crossList)
            {
                cross.draggable = false;
            }
        }
    }
}