using System;
using BestHTTP;
using BestHTTP.WebSocket;
using Manager;
using MyWebSocket.Response;
using Player;
using UnityEngine;

namespace MyWebSocket
{
    public class MyWebSocket : SingleTon<MyWebSocket>
    {
        public string uri = "webSocket://localhost:8080/web1/webSocket";

        public static WebSocket WebSocket;

        private bool _opened;

        private void Init()
        {
            //HTTPManager.Logger.Level = BestHTTP.Logger.Loglevels.All;
            WebSocket = new WebSocket(new Uri(uri));
#if !UNITY_WEBGL || UNITY_EDITOR
            WebSocket.StartPingThread = true;

#if !BESTHTTP_DISABLE_PROXY
            if (HTTPManager.Proxy != null)
            {
                WebSocket.OnInternalRequestCreated = (ws, internalRequest) =>
                    internalRequest.Proxy =
                        new HTTPProxy(HTTPManager.Proxy.Address, HTTPManager.Proxy.Credentials, false);
            }
#endif
#endif

            WebSocket.OnOpen += OnOpen;
            WebSocket.OnMessage += OnMessageReceived;
            WebSocket.OnError += OnError;
            WebSocket.OnClosed += OnClosed;
            WebSocket.OnBinary += OnBinaryMessageReceived;
        }

        public void Connect()
        {
            if (WebSocket == null)
            {
                Init();
            }

            if (WebSocket.IsOpen)
            {
                Debug.Log("webSocket already connected");
                return;
            }

            if (!_opened)
            {
                _opened = true;
                WebSocket?.Open();
                Debug.Log("webSocket connecting...");
            }
            else
            {
                Debug.Log("webSocket already call open");
            }
        }

        public void Send(string str)
        {
            WebSocket.Send(str);
        }

        public void Close()
        {
            WebSocket.Close();
        }

        #region MonoBehaviour

        protected override void Awake()
        {
            base.Awake();

            //HTTPManager.Logger.Level = BestHTTP.Logger.Loglevels.All;
            if (WebSocket == null)
            {
                Init();
            }
        }

        private void OnDestroy()
        {
            if (WebSocket != null)
            {
                WebSocket.Close();
                WebSocket = null;
            }
        }

        #endregion


        #region WebSocket事件回调

        /// <summary>
        /// 在与服务器建立连接时调用
        /// 在此事件之后，WebSocket 的 IsOpen 属性将为 True
        /// </summary>
        private void OnOpen(WebSocket ws)
        {
            Debug.Log("WebSocket Open!");
        }

        /// <summary>
        /// 在从服务器收到文本消息时调用
        /// </summary>
        private void OnMessageReceived(WebSocket ws, string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                Debug.Log(message);
                MyGameManager.Instance.NetWorkOperations.GetResponse(message);
            }
        }

        /// <summary>
        /// 在从服务器二进制消息时调用
        /// </summary>
        private void OnBinaryMessageReceived(WebSocket ws, byte[] message)
        {
            Debug.Log("Binary Message received from server. Length: " + message.Length);
        }

        /// <summary>
        /// 在与服务器关闭连接时调用
        /// </summary>
        private void OnClosed(WebSocket ws, ushort code, string message)
        {
            Debug.Log("WebSocket Closed!");
            _opened = false;
            WebSocket = null;
        }


        /// <summary>
        /// 在无法连接到服务器、发生内部错误或连接丢失时调用
        /// </summary>
        private void OnError(WebSocket ws, string error)
        {
            Debug.Log($"An error occured: <color=red>{error}</color>");
            _opened = false;
            WebSocket = null;
        }

        #endregion
    }
}