using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FairyGUI;
using Manager;
using Player;
using UI.Game;
using Unity.Netcode;
using UnityEngine;

namespace Tasks
{
    public class LinkLineTask : TaskUtil
    {
        private readonly List<GGraph> _points = new();

        private GGraph _firstPoint;
        private GGraph _secondPoint;
        private GTextField _successText;
        private GGraph _successTextBg;

        private readonly List<GGraph> _lines = new();
        private int _lineNum;

        protected override void Awake()
        {
            TaskPanelName = "LinkLines";
            base.Awake();
            _points.Clear();
            _points.Add(TaskUI.GetChild("p1").asGraph);
            _points.Add(TaskUI.GetChild("p2").asGraph);
            _points.Add(TaskUI.GetChild("p3").asGraph);

            foreach (var varPoint in _points)
            {
                varPoint.onClick.Add(() => { AsPointBeClicked(varPoint); });
            }

            _successText = TaskUI.GetChild("Success_Text").asTextField;
            _successTextBg = TaskUI.GetChild("Success_TextBG").asGraph;
        }

        protected override void InitTask()
        {
            _firstPoint = null;
            _secondPoint = null;
            
            foreach (var line in _lines)
            {
                TaskUI.RemoveChild(line);
                _lines.Remove(line);
            }

            _lineNum = 0;
            
            _successText.visible = false;
            _successTextBg.visible = false;
        }

        /// <summary>
        /// 检查两个点之间是否可以连线
        /// 只有p1和p2以及p2和p3之间可以连线
        /// </summary>
        /// <returns></returns>
        private bool CheckTwoPoint()
        {
            if (_firstPoint == null || _secondPoint == null)
            {
                return false;
            }

            return Mathf.Abs(_points.FindIndex(point => point.name == _firstPoint.name) -
                             _points.FindIndex(point => point.name == _secondPoint.name)) == 1;
        }

        private void AsPointBeClicked(GGraph graph)
        {
            if (_lineNum < _points.Count - 1)
            {
                if (_firstPoint == null)
                {
                    _firstPoint = graph;
                    _firstPoint.color = Color.red;
                }
                else
                {
                    _secondPoint = graph;
                    _secondPoint.color = Color.red;
                }

                if (_secondPoint != null)
                {
                    if (CheckTwoPoint())
                    {
                        DrawLine();
                        _lineNum++;
                    }

                    if (_firstPoint != null)
                    {
                        _firstPoint.color = Color.white;
                    }

                    if (_secondPoint != null)
                    {
                        _secondPoint.color = Color.white;
                    }

                    _firstPoint = null;
                    _secondPoint = null;
                }
            }


            if (_lineNum == _points.Count - 1)
            {
                IsSuccess = true;
                ShowSuccessText();
            }
        }

        private void DrawLine()
        {
            var line = new GGraph();
            line.DrawRect(15f, 15f, 0, Color.clear, Color.black);
            line.SetPivot(0f, 0.5f, true);
            line.xy = _firstPoint.xy;
            var twoPointsDir = _secondPoint.xy - _firstPoint.xy;
            line.width = twoPointsDir.magnitude;
            line.rotation = Mathf.Atan2(twoPointsDir.y, twoPointsDir.x) / Mathf.PI * 180;
            TaskUI.AddChild(line);
            _lines.Add(line);
        }

        private void ShowSuccessText()
        {
            _successText.visible = true;
            _successTextBg.visible = true;
            foreach (var point in _points)
            {
                point.onClick.Clear();
            }
        }
    }
}