using System.Collections;
using BestHTTP.WebSocket;
using FairyGUI;
using Manager;
using UI.Util;
using Unity.Netcode;
using Unity.Netcode.Transports.UNET;
using UnityEngine;
using UnityEngine.Serialization;

namespace UI.Boot
{
    /// <summary>
    /// 启动界面UI
    /// </summary>
    public class BootUIPanel : UIPanelUtil
    {
        [Header("开始界面视频渲染器纹理")] public Texture bootVideoRenderTexture;
        private GLoader _videoLoader;

        private GButton _quitButton;
        private GButton _registerButton;
        private GButton _loginButton;
        private GButton _serverButton;
        private GTextInput _ipInput;
        private GTextField _severState;

        public static TipPanel TipPanel;
        public static MatchingPanel MatchingPanel;
        private static RegisterPanel _registerPanel;
        private static LoginPanel _loginPanel;

        public static GComponent ChoosePanelComponent;
        public static ChoosePanel ChoosePanel;
        public static GComponent InfoPanelComponent;
        public static InfoPanel InfoPanel;
        private BlurFilter _blur;


        protected override void Awake()
        {
            base.Awake();

            ChoosePanelComponent ??= UIPackage.CreateObject("Boot", "ChoosePanel").asCom;
            InfoPanelComponent ??= UIPackage.CreateObject("Boot", "InfoPanel").asCom;

            ChoosePanel ??= new ChoosePanel(ChoosePanelComponent, UIRoot);
            InfoPanel ??= new InfoPanel(InfoPanelComponent);

            TipPanel ??= new TipPanel();
            MatchingPanel ??= new MatchingPanel();
            _loginPanel ??= new LoginPanel(UIRoot);
            _registerPanel ??= new RegisterPanel(UIRoot);

            _blur = new BlurFilter
            {
                blurSize = 0.25f
            };
        }

        private void Start()
        {
            _quitButton = GetButton("Button_Quit");
            _registerButton = GetButton("Button_Register");
            _loginButton = GetButton("Button_Login");
            _serverButton = GetButton("Button_server");
            _ipInput = UIRoot.GetChild("Input_ip").asTextInput;
            _severState = UIRoot.GetChild("connect").asTextField;
            _videoLoader = UIRoot.GetChild("bg").asLoader;
            _videoLoader.texture = new NTexture(bootVideoRenderTexture);

            _quitButton?.onClick.Add(() => { StartCoroutine(QuitGame()); });
            _registerButton?.onClick.Add(() =>
            {
                _registerPanel.Show();
                UIRoot.filter = _blur;
            });
            _loginButton?.onClick.Add(() =>
            {
                _loginPanel.Show();
                UIRoot.filter = _blur;
                MyGameManager.Instance.serverIP = _ipInput.text;
            });

            _ipInput.text = MyGameManager.Instance.serverIP;
            _serverButton.title = MyGameManager.Instance.isServer ? "主机" : "客户端";
            _serverButton.onClick.Add(() =>
            {
                MyGameManager.Instance.isServer = !MyGameManager.Instance.isServer;
                _serverButton.title = MyGameManager.Instance.isServer ? "主机" : "客户端";
            });
        }

        private static IEnumerator QuitGame()
        {
            if (MyWebSocket.MyWebSocket.WebSocket == null)
            {
                UIOperationUtil.QuitGame();
                yield break;
            }

            MyWebSocket.MyWebSocket.Instance.Close();
            while (MyWebSocket.MyWebSocket.WebSocket != null)
            {
                yield return null;
            }

            UIOperationUtil.QuitGame();
        }

        private void Update()
        {
            if (MyWebSocket.MyWebSocket.WebSocket == null)
            {
                return;
            }

            switch (MyWebSocket.MyWebSocket.WebSocket.State)
            {
                case WebSocketStates.Connecting:
                    _severState.SetVar("isConnect", "未连接").FlushVars();
                    break;
                case WebSocketStates.Open:
                    _severState.SetVar("isConnect", "已连接").FlushVars();
                    break;
                case WebSocketStates.Closed:
                case WebSocketStates.Closing:
                case WebSocketStates.Unknown:
                    _severState.SetVar("isConnect", "未连接").FlushVars();
                    break;
                default:
                    _severState.SetVar("isConnect", "未连接").FlushVars();
                    break;
            }
        }
    }
}