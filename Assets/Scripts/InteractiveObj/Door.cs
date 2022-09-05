using System;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

namespace InteractiveObj
{
    public enum DoorMoveDirection
    {
        Left,
        Right,
        Top,
        Down
    }

    public class Door : MonoBehaviour
    {
        public DoorMoveDirection direction;
        public float moveDistance;
        public float duration;
        public bool isMoved;

        private TweenerCore<Vector3, Vector3, VectorOptions> _moveTween;

        private void Awake()
        {
            switch (direction)
            {
                case DoorMoveDirection.Left:
                    _moveTween = transform.DOMoveX(transform.position.x - moveDistance, duration);
                    break;
                case DoorMoveDirection.Right:
                    _moveTween = transform.DOMoveX(transform.position.x + moveDistance, duration);
                    break;
                case DoorMoveDirection.Down:
                    _moveTween = transform.DOMoveY(transform.position.y - moveDistance, duration);
                    break;
                case DoorMoveDirection.Top:
                    _moveTween = transform.DOMoveY(transform.position.y + moveDistance, duration);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _moveTween.SetAutoKill(false);
        }

        public void DoorMove(bool targetMoveState)
        {
            if (targetMoveState == isMoved)
            {
                return;
            }

            if (targetMoveState)
            {
                _moveTween.PlayForward();
            }
            else
            {
                _moveTween.PlayBackwards();
            }

            isMoved = targetMoveState;
        }
    }
}