using System.Collections.Generic;
using FairyGUI;
using UnityEngine;

namespace Tasks
{
    public class FeedCatTask : TaskUtil
    {
        private readonly List<GLoader> _catFoodsList = new();
        private GImage _cat;
        private GTextField _successText;
        private GGraph _successTextBg;

        private bool _isDragging;
        private int _feedNum;

        protected override void Awake()
        {
            TaskPanelName = "FeedCat";

            base.Awake();
            _catFoodsList.Clear();
            _catFoodsList.Add(TaskUI.GetChild("Img_CatFood1").asLoader);
            _catFoodsList.Add(TaskUI.GetChild("Img_CatFood2").asLoader);
            _catFoodsList.Add(TaskUI.GetChild("Img_CatFood3").asLoader);
            foreach (var catFoodImg in _catFoodsList)
            {
                catFoodImg.pivot = new Vector2(0.5f, 0.5f);
                catFoodImg.pivotAsAnchor = true;
                catFoodImg.draggable = true;
                catFoodImg.dragBounds = new Rect(560,240,800,600);
                catFoodImg.onDragStart.Add(() =>
                {
                    if (!_isDragging)
                    {
                        _isDragging = true;
                    }
                    else
                    {
                        catFoodImg.StopDrag();
                    }
                });
                catFoodImg.onDragEnd.Add(() =>
                {
                    CatFoodDragEnd(catFoodImg);
                    _isDragging = false;
                });
            }


            _cat = TaskUI.GetChild("Img_Cat").asImage;
            _cat.pivot = new Vector2(0.5f, 0.5f);
            _cat.pivotAsAnchor = true;

            _successText = TaskUI.GetChild("Success_Text").asTextField;
            _successTextBg = TaskUI.GetChild("Success_TextBG").asGraph;
        }

        protected override void InitTask()
        {
            base.InitTask();
            _feedNum = 0;
            _isDragging = false;
            _successText.visible = false;
            _successTextBg.visible = false;
        }

        private void CatFoodDragEnd(GObject image)
        {
            var distanceXY = image.xy - _cat.xy;
            if (distanceXY.x < image.width / 2f + _cat.width / 2f && distanceXY.y < image.height / 2f + _cat.height / 2f)
            {
                image.draggable = false;
                image.visible = false;
                _feedNum++;
            }

            if (_feedNum == _catFoodsList.Count)
            {
                IsSuccess = true;
                ShowSuccessText();
            }
        }

        private void ShowSuccessText()
        {
            _successText.visible = true;
            _successTextBg.visible = true;

            foreach (var catFoodImg in _catFoodsList)
            {
                catFoodImg.draggable = false;
            }
        }
    }
}