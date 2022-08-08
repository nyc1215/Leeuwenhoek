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
        #region 游戏规则相关

        [Header("游戏规则相关")] [Tooltip("是否是狼人")] public bool isImposter;

        [Space(10)] [Header("操控")] [Tooltip("wasd移动操控")]
        public InputAction inputWasd;

        [Tooltip("击杀")] public InputAction inputKill;

        #endregion

        #region 角色状态

        [Space(10)] [Header("角色状态")] [Tooltip("移动速度")]
        public float moveSpeed;

        [Tooltip("是否允许操控")] public bool hasControl;
        [Tooltip("是否死亡")] public bool isDead;

        #endregion

        public static MyPlayerController LocalPlayerController;

        private Rigidbody _playerRigidbody;
        private Transform _playerTransform;
        private Animator _animator;
        private Collider _collider;
        private static readonly int AnimatorParamSpeed = Animator.StringToHash("speed");
        private static readonly int AnimatorParamIsDead = Animator.StringToHash("isDead");

        private MyPlayerController _target;
        private Vector2 _moveInput;


        private void Awake()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _playerTransform = transform.GetChild(0);
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
            _target = null;

            inputKill.performed += KillTarget;
        }

        private void Start()
        {
            if (hasControl)
            {
                LocalPlayerController = this;
            }

            if (!hasControl)
            {
                return;
            }
        }

        private void Update()
        {
            if (!hasControl)
            {
                return;
            }

            _moveInput = inputWasd.ReadValue<Vector2>();
            if (_moveInput.x != 0)
            {
                _playerTransform.localScale = new Vector2(Mathf.Sign(_moveInput.x), 1);
            }

            _animator.SetFloat(AnimatorParamSpeed, _moveInput.magnitude);
        }

        private void FixedUpdate()
        {
            _playerRigidbody.velocity = _moveInput * moveSpeed;
        }

        private void OnEnable()
        {
            inputWasd.Enable();
            inputKill.Enable();
        }

        private void OnDisable()
        {
            inputWasd.Disable();
            inputKill.Disable();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var targetPlayer = other.GetComponent<MyPlayerController>();
                if (!isImposter)
                {
                    return;
                }

                if (targetPlayer.isImposter)
                {
                    return;
                }

                _target = targetPlayer;
            }
        }

        private void KillTarget(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.phase == InputActionPhase.Performed)
            {
                if (_target == null)
                {
                    return;
                }

                if (_target.isDead)
                {
                    return;
                }

                _target.Die();
                _target = null;
            }
        }

        private void Die()
        {
            isDead = true;
            _animator.SetBool(AnimatorParamIsDead, true);
            _collider.enabled = false;
        }
    }
}