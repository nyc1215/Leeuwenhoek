using FairyGUI;
using UnityEngine;

namespace Tasks
{
    public class MoveTelescopeBinoculars : TaskUtil
    {
        private GLoader _telescopeBinocular;

        private GGraph _startPoint;
        private GGraph _endPoint;
        private GTextField _successText;

        protected override void Awake()
        {
            TaskPanelName = "MoveTelescopeBinoculars";

            base.Awake();

            _telescopeBinocular = TaskUI.GetChild("Loader_TelescopeBinoculars").asLoader;
            _endPoint = TaskUI.GetChild("EndPoint").asGraph;
            _startPoint = TaskUI.GetChild("StartPoint").asGraph;
            _successText = TaskUI.GetChild("Success_Text").asTextField;

            _telescopeBinocular.onDragMove.Add(OnTelescopeBinocularBeDragged);
            _telescopeBinocular.onDragEnd.Add(OnTelescopeBinocularDragEnd);
        }

        protected override void InitTask()
        {
            _telescopeBinocular.draggable = true;
            _successText.visible = false;
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
                IsSuccess = true;
                _telescopeBinocular.draggable = false;
                _successText.visible = true;
            }
        }
    }
}