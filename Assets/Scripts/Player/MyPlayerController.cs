using System;
using System.Collections;
using System.Collections.Generic;
using Manager;
using MyWebSocket.Request;
using MyWebSocket.Response;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class MyPlayerController : MonoBehaviour
    {
        #region 游戏规则相关变量

        [Header("游戏规则相关")] [Tooltip("是否是狼人")] public bool isImposter;

        [Space(10)] [Header("操控")] [Tooltip("wasd移动操控")]
        public InputAction inputWasd;

        [Tooltip("击杀")] public InputAction inputKill;

        [Tooltip("报告")] public InputAction inputReport;
        public static List<Transform> AllBodies;
        [Tooltip("寻找尸体的时候忽略的层")] public LayerMask ignoreForBody;

        #endregion

        #region 角色状态变量

        [Space(10)] [Header("角色状态")] [Tooltip("移动速度")]
        public float moveSpeed;

        [Tooltip("是否允许操控")] public bool hasControl;
        [Tooltip("是否死亡")] public bool isDead;
        [Tooltip("角色尸体预制体")] public GameObject bodyPrefab;
        [Tooltip("是否隐藏")] public bool isHide;

        #endregion

        #region 角色显示相关变量

        [Space(10)] [Header("角色显示相关")] [Tooltip("角色身体精灵渲染器")]
        public SpriteRenderer playerSpriteRenderer;

        [Tooltip("角色其他部分精灵渲染器")] public SpriteRenderer playerPartSpriteRenderer;

        #endregion

        #region 游戏管理系统相关变量

        [Space(10)] [Header("游戏管理系统")] [Tooltip("是否是本地玩家")]
        public bool isLocalClient;

        public static MyPlayerController LocalPlayerController;

        #endregion

        #region 私有变量

        private readonly Hashtable _requestPool = new();

        private Rigidbody _playerRigidbody;
        private Transform _playerTransform;
        private Animator _animator;
        private Collider _collider;
        private static readonly int AnimatorParamSpeed = Animator.StringToHash("speed");
        private static readonly int AnimatorParamIsDead = Animator.StringToHash("isDead");

        private List<MyPlayerController> _targets;
        private Vector2 _moveInput;

        private List<Transform> _bodiesFound;

        #endregion

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

            inputKill.performed += KillTarget;
            inputReport.performed += Report;
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
            _animator.SetFloat(AnimatorParamSpeed, _moveInput.magnitude);
            if (_moveInput.x != 0)
            {
                _playerTransform.localScale = new Vector2(Mathf.Sign(_moveInput.x), 1);
            }

            if (AllBodies.Count > 0)
            {
                BodySearch();
            }

            if (MyGameManager.Instance != null)
            {
                if (MyGameManager.Instance.allPlayers?.Count > 0)
                {
                    HideOtherPlayers();
                }
            }
        }

        private void FixedUpdate()
        {
            _playerRigidbody.velocity = _moveInput * moveSpeed;
        }

        private void OnEnable()
        {
            inputWasd.Enable();
            inputKill.Enable();
            inputReport.Enable();
        }

        private void OnDisable()
        {
            inputWasd.Disable();
            inputKill.Disable();
            inputReport.Disable();
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

        #region 服务器通信

        public void SendRequest<T>(T request) where T : RequestUtil
        {
            var t = typeof(T);
            if (MyWebSocket.MyWebSocket.Instance.IsOpen)
            {
                MyWebSocket.MyWebSocket.Instance.Send(request.ToJson());
                if (t == typeof(RequestInvite) || t == typeof(RequestAddRoom))
                {
                    return;
                }

                _requestPool.Add(request.RequestID, request);
            }
            else
            {
                Debug.LogWarning("Sever is not connected!!!");
            }
        }

        public void GetResponse(string json)
        {
            ResponseUtil responseUtil = new(json);

            if (!Enum.TryParse(responseUtil.NowResponseType, out ResponseType nowTypeOfResponse))
            {
                Debug.Log("string to ResponseType failed");
                return;
            }

            switch (nowTypeOfResponse)
            {
                case ResponseType.Error:
                    ResponseErrorWork((ResponseError)responseUtil);
                    break;
                case ResponseType.Response:
                    ResponseWork((Response)responseUtil);
                    break;
                case ResponseType.SynchronousData:
                    SynchronousData((ResponseSynchronousData)responseUtil);
                    break;
                default:
                    throw new System.InvalidCastException();
            }
        }

        private void SynchronousData(ResponseSynchronousData responseSynchronousData)
        {
        }

        private void ResponseErrorWork(ResponseError responseError)
        {
            if (!_requestPool.ContainsKey(responseError.RequestID))
            {
                return;
            }

            _requestPool.Remove(responseError.RequestID);
        }

        private void ResponseWork(Response response)
        {
            if (!_requestPool.ContainsKey(response.RequestID))
            {
                return;
            }

            _requestPool.Remove(response.RequestID);
        }

        #endregion

        private void HideOtherPlayers()
        {
            foreach (var otherPlayerController in MyGameManager.Instance.allPlayers)
            {
                if (otherPlayerController.isLocalClient)
                {
                    return;
                }

                var myPlayerPos = transform.position;
                var otherPlayerPos = otherPlayerController.transform.position;
                var ray = new Ray(myPlayerPos, otherPlayerPos - myPlayerPos);
                Debug.DrawLine(myPlayerPos, otherPlayerPos, Color.green);

                if (Physics.Raycast(ray, out var hit, 1000f, ~ignoreForBody))
                {
                    if (hit.transform == otherPlayerController.transform)
                    {
                        //Debug.Log(hit.transform.name);
                        //Debug.Log($"_bodiesFound :{_bodiesFound.Count}");
                        otherPlayerController.isHide = false;
                        otherPlayerController.HideOrShowPlayerSelf();
                    }
                    else
                    {
                        otherPlayerController.isHide = true;
                        otherPlayerController.HideOrShowPlayerSelf();
                    }
                }
            }
        }

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


        private void KillTarget(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.phase == InputActionPhase.Performed)
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

        private void Report(InputAction.CallbackContext callbackContext)
        {
            if (callbackContext.phase == InputActionPhase.Performed)
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

        private void HideOrShowPlayerSelf()
        {
            playerSpriteRenderer.enabled = isHide;
            playerPartSpriteRenderer.enabled = isHide;
        }
    }
}