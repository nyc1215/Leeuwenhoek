using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace Tasks
{
    public class FixRobot : TaskUtil
    {
        private GTextField _successText;

        private const int AllPointsNum = 10;
        private int _clickNum;

        protected override void Awake()
        {
            TaskPanelName = "FixRobot";

            base.Awake();
            
            for (var i = 1; i <= AllPointsNum; i++)
            {
                var pointGraph = TaskUI.GetChild($"p{i}").asGraph;
                pointGraph.onClick.Add(() =>
                {
                    PointBeClicked(pointGraph);
                });
            }
            
            _successText = TaskUI.GetChild("Success_Text").asTextField;
        }

        protected override void InitTask()
        {
            _clickNum = 0;
            _successText.visible = false;
        }

        private void PointBeClicked(GGraph point)
        {
            point.color = Color.red;
            _clickNum++;
            point.onClick.Clear();

            if (_clickNum == AllPointsNum)
            {
                IsSuccess = true;
                ShowSuccessText();
            }
        }

        private void ShowSuccessText()
        {
            _successText.visible = true;
        }
    }
}