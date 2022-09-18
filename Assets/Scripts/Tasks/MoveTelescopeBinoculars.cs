using FairyGUI;
using UnityEngine;

namespace Tasks
{
    public class MoveTelescopeBinoculars : TaskUtil
    {
        private GLoader _telescopeBinocular;

        private GLoader _startPoint;
        private GLoader _endPoint;
        private GTextField _successText;
        private GGraph _successTextBg;

        protected override void Awake()
        {
            TaskPanelName = "MoveTelescopeBinoculars";

            base.Awake();

            _telescopeBinocular = TaskUI.GetChild("Loader_TelescopeBinoculars").asLoader;
            _endPoint = TaskUI.GetChild("EndPoint").asLoader;
            _startPoint = TaskUI.GetChild("StartPoint").asLoader;
            _successText = TaskUI.GetChild("Success_Text").asTextField;

            _telescopeBinocular.onDragMove.Add(OnTelescopeBinocularBeDragged);
            _telescopeBinocular.onDragEnd.Add(OnTelescopeBinocularDragEnd);
            
            _successTextBg = TaskUI.GetChild("Success_TextBG").asGraph;
        }

        protected override void InitTask()
        {
            base.InitTask();
            _telescopeBinocular.draggable = true;
            _successText.visible = false;
            _successTextBg.visible = false;
        }

        private void OnTelescopeBinocularBeDragged()
        {
            _telescopeBinocular.y = 300f;
            _telescopeBinocular.x = Mathf.Clamp(Stage.inst.touchPosition.x - 560f, _startPoint.x, _endPoint.x);
        }

        private void OnTelescopeBinocularDragEnd()
        {
            if (_telescopeBinocular.x >= _endPoint.x - _endPoint.width / 2f)
            {
                isSuccess = true;
                _telescopeBinocular.draggable = false;
                _successText.visible = true;
                _successTextBg.visible = true;
            }
        }
    }
}