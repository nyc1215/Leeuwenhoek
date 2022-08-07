using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class MyPlayerController : MonoBehaviour
    {
        private Rigidbody _playerRigidbody;
        private Transform _playerTransform;

        public InputAction inputWasd;

        private Vector2 _moveInput;
        public float moveSpeed;

        private void Awake()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _playerTransform = transform.GetChild(0);
        }

        private void Update()
        {
            _moveInput = inputWasd.ReadValue<Vector2>();
            if (_moveInput.x != 0)
            {
                _playerTransform.localScale = new Vector2(Mathf.Sign(_moveInput.x), 1);
            }
        }

        private void FixedUpdate()
        {
            _playerRigidbody.velocity = _moveInput * moveSpeed;
        }

        private void OnEnable()
        {
            inputWasd.Enable();
        }

        private void OnDisable()
        {
            inputWasd.Disable();
        }
    }
}