using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using InteractiveObj;
using Manager;
using Tasks;
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
        public TaskUtil nowTask;
        public SewerForImposter nowSewer;

        #endregion

        #region 角色显示

        [Space(10)] [Header("角色显示相关")] [Tooltip("角色身体精灵渲染器")]
        public SpriteRenderer playerSpriteRenderer;

        [Tooltip("角色其他部分精灵渲染器")] public SpriteRenderer playerPartSpriteRenderer;
        [Tooltip("角色主摄像机")] public Camera playerMainCamera;
        [Tooltip("角色在阴影下需要隐藏的物体")] public List<Renderer> objsToHide;
        [Tooltip("隐藏物体时候忽略的层")] public LayerMask ignoreForHide;

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

        public Action ChangeSceneCallBack;

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

            MyGameManager.Instance.localPlayerController = this;
            DontDestroyOnLoad(this);
        }

        private void Start()
        {
            inputKill.performed += KillTarget;
            inputReport.performed += Report;
            ChangeSceneCallBack += OnGameSceneInit;
        }

        public override void OnDestroy()
        {
            if (IsServer || IsHost)
            {
                foreach (var playerController in MyGameManager.Instance.allPlayers)
                {
                    Destroy(playerController.gameObject);
                }
            }

            base.OnDestroy();
        }


        public void OnEnable()
        {
            inputWasd.Enable();
            inputReport.Enable();
            inputKill.Enable();
        }

        public void OnDisable()
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

            PlayerSearch();

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

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _targets.Sort((player1, player2) =>
                {
                    var localPlayerPosition = transform.position;
                    var player1Distance = (player1.transform.position - localPlayerPosition).magnitude;
                    var player2Distance = (player2.transform.position - localPlayerPosition).magnitude;
                    return -player1Distance.CompareTo(player2Distance);
                });
            }
        }

        private void LateUpdate()
        {
            if (IsOwner)
            {
                if (MyGameManager.CompareScene(MyGameManager.Instance.uiJumpData.gameMenu))
                {
                    var localPlayerPosition = transform.position;
                    if (playerMainCamera == null)
                    {
                        playerMainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
                    }

                    var playerMainCameraTransform = playerMainCamera.transform;
                    playerMainCameraTransform.position = new Vector3(localPlayerPosition.x, localPlayerPosition.y, playerMainCameraTransform.position.z);
                }
            }
        }

        #endregion

        #region NetCode

        public override void OnNetworkSpawn()
        {
            MyGameManager.Instance.allPlayers.Add(this);
            if (!IsOwner)
            {
                _playerLight2D.enabled = false;
            }
            else
            {
                if (MyGameManager.CompareScene(MyGameManager.Instance.uiJumpData.roomMenu))
                {
                    _playerLight2D.shadowIntensity = 0.7f;
                    _playerLight2D.intensity = 1.5f;
                    inputReport.Disable();
                    inputKill.Disable();
                }

                StartCoroutine(CheckAllPlayersIsHadSpawn());
            }

            transform.position = GameObject.Find("StartPoint").transform.position;
        }

        private IEnumerator CheckAllPlayersIsHadSpawn()
        {
            while (MyGameManager.Instance.allPlayers.Count < MyGameManager.Instance.PlayerListData.PlayerList.Count)
            {
                yield return null;
            }

            MyGameManager.Instance.RandomSetImposter();

            foreach (var playerController in MyGameManager.Instance.allPlayers)
            {
                ChangePlayerImposterClientRpc(playerController.NetworkObject.NetworkObjectId, playerController.isImposter);
            }
        }

        [ClientRpc]
        private void ChangePlayerImposterClientRpc(ulong networkObjectId, bool beImposter)
        {
            if (!IsServer)
            {
                foreach (var playerController in MyGameManager.Instance.allPlayers.Where(playerController => playerController.NetworkObject.NetworkObjectId == networkObjectId))
                {
                    playerController.isImposter = beImposter;
                    Debug.Log($"networkObjectId: {networkObjectId} beImposter: {beImposter}");
                }
            }
        }

        #endregion

        #region GamePlay

        private void PlayerSearch()
        {
            if (IsOwner)
            {
                foreach (var otherPlayerController in MyGameManager.Instance.allPlayers)
                {
                    if (otherPlayerController == this)
                    {
                        continue;
                    }

                    var playerPos = transform.position;
                    var otherPlayerPos = otherPlayerController.transform.position;
                    var ray = new Ray(playerPos, otherPlayerPos - playerPos);
                    Debug.DrawLine(playerPos, otherPlayerPos, Color.magenta);

                    if (Physics.Raycast(ray, out var hit, 1000f, ~ignoreForHide))
                    {
                        otherPlayerController.ChangeAllComponentsNeedToHide(hit.transform == otherPlayerController.transform);
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

            //transform.position = _targets[^1].transform.position;
            _targets[^1].Die();
            _targets.RemoveAt(_targets.Count - 1);
        }

        public void KillTarget()
        {
            if (_targets.Count == 0)
            {
                return;
            }

            if (_targets[^1].isDead)
            {
                return;
            }

            //transform.position = _targets[^1].transform.position;
            _targets[^1].Die();
            _targets.RemoveAt(_targets.Count - 1);
        }


        private void Die()
        {
            isDead = true;
            _animator.SetBool(AnimatorParamIsDead, true);
            _collider.enabled = false;

            gameObject.layer = LayerMask.NameToLayer("Ghost") == -1 ? 9 : LayerMask.NameToLayer("Ghost");

            if (!isImposter)
            {
                MyGameManager.Instance.goodPlayerNum--;
            }

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

        public void Report()
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

        public void DoTask()
        {
            if (nowTask != null)
            {
                nowTask.StartTask();
            }
        }

        #endregion

        private void OnGameSceneInit()
        {
            if (IsOwner)
            {
                if (MyGameManager.CompareScene(MyGameManager.Instance.uiJumpData.gameMenu))
                {
                    GetComponent<MyPlayerNetwork>().ChangeTopTextColor(false);
                    GetComponent<MyPlayerNetwork>().ChangeVoiceIconShow(false);
                    _playerLight2D.enabled = true;
                    inputReport.Enable();
                    inputKill.Enable();

                    if (playerMainCamera == null)
                    {
                        playerMainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
                        playerMainCamera.targetTexture = RenderTexture.active;
                    }

                    transform.position = GameObject.Find("StartPoint").transform.position;
                }
            }
        }

        private void ChangeAllComponentsNeedToHide(bool isShow)
        {
            foreach (var objRenderer in objsToHide)
            {
                objRenderer.enabled = isShow;
                Debug.Log($"obj: {objRenderer.name} show: {isShow}");
            }
        }
    }
}