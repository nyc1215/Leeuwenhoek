using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;

namespace UI.Util
{
    public class MiniMap : MonoBehaviour
    {
        private Transform _miniMapCameraTransform;

        private void Awake()
        {
            _miniMapCameraTransform = transform;
        }

        private void LateUpdate()
        {
            if (MyGameManager.Instance.localPlayerController == null)
            {
                return;
            }

            var localPlayerPos = MyGameManager.Instance.localPlayerController.transform.position;
            var miniMapPosition = _miniMapCameraTransform.position;
            miniMapPosition = new Vector3(localPlayerPos.x, miniMapPosition.y, miniMapPosition.z);
            _miniMapCameraTransform.position = miniMapPosition;
        }
    }
}