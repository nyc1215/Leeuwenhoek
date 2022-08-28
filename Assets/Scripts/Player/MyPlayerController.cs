using System.Collections.Generic;
using Manager;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

namespace Player
{
    public class MyPlayerController : NetworkBehaviour
    {
        #region 游戏规则和操作

        [Header("游戏规则相关")] [Tooltip("是否是狼人")] public bool isImposter;

        [Space(10)] [Header("操控")] [Tooltip("wasd移动操控")]
        public InputAction inputWasd;

        [Tooltip("摇杆移动操控")] public JoyStickOutputXY JoyStickOutput;

        [Tooltip("击杀")] public InputAction inputKill;

        [Tooltip("报告")] public InputAction inputReport;
        public static List<Transform> AllBodies;
        [Tooltip("寻找尸体的时候忽略的层")] public LayerMask ignoreForBody;

        #endregion

        #region 角色状态

        [Space(10)] [Header("角色状态")] [Tooltip("移动速度")]
        public float moveSpeed;

        [Tooltip("是否死亡")] public bool isDead;
        [Tooltip("角色尸体预制体")] public GameObject bodyPrefab;

        #endregion

        #region 角色显示

        [Space(10)] [Header("角色显示相关")] [Tooltip("角色身体精灵渲染器")]
        public SpriteRenderer playerSpriteRenderer;

        [Tooltip("角色其他部分精灵渲染器")] public SpriteRenderer playerPartSpriteRenderer;

        #endregion

        private Light2D _playerLight2D;
        private Rigidbody _playerRigidbody;
        private Transform _playerTransform;
        private Animator _animator;
        private Collider _collider;
        private static readonly int AnimatorParamSpeed = Animator.StringToHash("speed");
        private static readonly int AnimatorParamIsDead = Animator.StringToHash("isDead");

        private List<MyPlayerController> _targets;
        private Vector2 _moveInput;

        private List<Transform> _bodiesFound;

        #region MonoBehaviour

        private void Awake()
        {
            _playerRigidbody = GetComponent<Rigidbody>();
            _playerTransform = transform.GetChild(0);
            _animator = GetComponent<Animator>();
            _collider = GetComponent<Collider>();
            _targets = new List<MyPlayerController>();
            AllBodies = new List<Transform>();
            _bodiesFound = new List<Transform>();
            _playerLight2D = transform.GetChild(1).GetComponent<Light2D>();
        }

        private void Start()
        {
            inputKill.performed += KillTarget;
            inputReport.performed += Report;
        }


        private void OnEnable()
        {
            inputWasd.Enable();
            inputReport.Enable();
            inputKill.Enable();
        }

        private void OnDisable()
        {
            inputWasd.Disable();
            inputReport.Disable();
            inputKill.Disable();
        }

        private void Update()
        {
            if (!IsLocalPlayer)
            {
                return;
            }

            _moveInput = inputWasd.ReadValue<Vector2>();
            if (!JoyStickOutput.IsZero())
            {
                _moveInput = JoyStickOutput.GetVector2().normalized;
            }

            _animator.SetFloat(AnimatorParamSpeed, _moveInput.magnitude);

            if (_moveInput.x != 0)
            {
                _playerTransform.localScale = new Vector2(Mathf.Sign(_moveInput.x), 1);
            }

            if (AllBodies.Count > 0)
            {
                BodySearch();
            }
        }

        private void FixedUpdate()
        {
            _playerRigidbody.velocity = _moveInput * moveSpeed;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var targetPlayer = other.GetComponent<MyPlayerController>();
                if (isImposter)
                {
                    if (targetPlayer.isImposter)
                    {
                        return;
                    }

                    _targets.Add(targetPlayer);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                var targetPlayer = other.GetComponent<MyPlayerController>();
                if (_targets.Contains(targetPlayer))
                {
                    _targets.Remove(targetPlayer);
                }
            }
        }

        #endregion

        #region NetCode

        public override void OnNetworkSpawn()
        {
            if (IsLocalPlayer)
            {
                MyGameManager.Instance.localPlayerController = this;
            }

            if (!IsOwner)
            {
                _playerLight2D.enabled = false;
            }
            else
            {
                MyGameManager.Instance.allPlayers.Add(this);
                if (MyGameManager.CompareScene(MyGameManager.Instance.uiJumpData.roomMenu))
                {
                    _playerLight2D.enabled = false;
                    inputReport.Disable();
                    inputKill.Disable();
                    transform.position = GameObject.Find("StartPoint").transform.position;
                }
            }
        }

        #endregion

        private void BodySearch()
        {
            //Debug.Log($"AllBodies :{AllBodies.Count}");
            foreach (var body in AllBodies)
            {
                var playerPos = transform.position;
                var bodyPos = body.position;
                var ray = new Ray(playerPos, bodyPos - playerPos);
                Debug.DrawLine(playerPos, bodyPos, Color.cyan);

                if (Physics.Raycast(ray, out var hit, 1000f, ~ignoreForBody))
                {
                    if (hit.transform == body)
                    {
                        //Debug.Log(hit.transform.name);
                        //Debug.Log($"_bodiesFound :{_bodiesFound.Count}");
                        if (_bodiesFound.Contains(body.transform))
                        {
                            return;
                        }

                        _bodiesFound.Add(body.transform);
                    }
                    else
                    {
                        _bodiesFound.Remove(body.transform);
                    }
                }
            }
        }


        private void KillTarget(InputAction.CallbackContext context)
        {
            if (_targets.Count == 0)
            {
                return;
            }

            if (_targets[^1].isDead)
            {
                return;
            }

            transform.position = _targets[^1].transform.position;
            _targets[^1].Die();
            _targets.RemoveAt(_targets.Count - 1);
        }

        private void Die()
        {
            isDead = true;
            _animator.SetBool(AnimatorParamIsDead, true);
            _collider.enabled = false;

            gameObject.layer = LayerMask.NameToLayer("Ghost") == -1 ? 9 : LayerMask.NameToLayer("Ghost");

            var trans = transform;
            var temPlayerBody = Instantiate(bodyPrefab, trans.position, trans.rotation)
                .GetComponent<MyPlayerBody>();
            temPlayerBody.SetColor(playerSpriteRenderer.color);
        }

        private void Report(InputAction.CallbackContext context)
        {
            if (_bodiesFound == null)
            {
                return;
            }

            if (_bodiesFound.Count == 0)
            {
                return;
            }

            var tempBody = _bodiesFound[^1];
            AllBodies.Remove(tempBody);
            _bodiesFound.Remove(tempBody);
            tempBody.GetComponent<MyPlayerBody>().Report();
        }
    }
}