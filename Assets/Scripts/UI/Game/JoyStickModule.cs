using FairyGUI;
using Manager;
using UnityEngine;

namespace UI.Game
{
    public class JoyStickModule : EventDispatcher
    {
        private readonly float _initX;
        private readonly float _initY;
        private readonly GButton _button;
        private readonly GObject _touchArea;
        private int _touchId;
        private GTweener _tweener;

        public EventListener onMove { get; private set; }
        public EventListener onEnd { get; set; }

        private int radius { get; set; }

        public JoyStickModule(GComponent mainView)
        {
            onMove = new EventListener(this, "onMove");
            onEnd = new EventListener(this, "onEnd");

            _button = mainView.GetChild("joystick").asButton;
            _button.changeStateOnClick = false;
            _touchArea = mainView.GetChild("joystick_touch");

            _initX = _touchArea.x;
            _initY = _touchArea.y;
            _touchId = -1;
            radius = 150;

            _touchArea.onTouchBegin.Add(OnTouchBegin);
            _touchArea.onTouchMove.Add(OnTouchMove);
            _touchArea.onTouchEnd.Add(OnTouchEnd);
        }

        private void OnTouchBegin(EventContext context)
        {
            if (_touchId != -1)
            {
                return;
            }

            var evt = (InputEvent)context.data;
            _touchId = evt.touchId;

            if (_tweener != null)
            {
                _tweener.Kill();
                _tweener = null;
            }

            var touchPos = GRoot.inst.GlobalToLocal(new Vector2(evt.x, evt.y));
            _button.selected = true;

            var touchPosXClamp = Mathf.Clamp(touchPos.x, _touchArea.x - _touchArea.width / 2,
                _touchArea.x + _touchArea.width / 2);
            var touchPosYClamp = Mathf.Clamp(touchPos.y, _touchArea.y - _touchArea.height / 2,
                _touchArea.y + _touchArea.height / 2);

            _button.SetXY(touchPosXClamp, touchPosYClamp);

            context.CaptureTouch();
        }

        private void OnTouchEnd(EventContext context)
        {
            var inputEvt = (InputEvent)context.data;
            if (_touchId == -1 || inputEvt.touchId != _touchId)
            {
                return;
            }

            _touchId = -1;
            _tweener = _button.TweenMove(new Vector2(_initX, _initY), 0.3f)
                .OnComplete(() =>
                    {
                        _tweener = null;
                        _button.selected = false;
                    }
                );
            onEnd.Call(new JoyStickOutputXY(0f, 0f));
        }

        private void OnTouchMove(EventContext context)
        {
            var evt = (InputEvent)context.data;
            if (_touchId == -1 || evt.touchId != _touchId)
            {
                return;
            }

            var pt = GRoot.inst.GlobalToLocal(new Vector2(evt.x, evt.y));
            var buttonX = pt.x;
            var buttonY = pt.y;
            var offsetX = buttonX - _initX;
            var offsetY = buttonY - _initY;

            var rad = Mathf.Atan2(offsetY, offsetX);

            var maxX = radius * Mathf.Cos(rad);
            var maxY = radius * Mathf.Sin(rad);
            if (Mathf.Abs(offsetX) > Mathf.Abs(maxX))
            {
                buttonX = _initX + maxX;
            }

            if (Mathf.Abs(offsetY) > Mathf.Abs(maxY))
            {
                buttonY = _initY + maxY;
            }

            _button.SetXY(buttonX, buttonY);

            var joyStickOutputXY = new JoyStickOutputXY(offsetX, -offsetY);

            onMove.Call(joyStickOutputXY);
        }
    }
}