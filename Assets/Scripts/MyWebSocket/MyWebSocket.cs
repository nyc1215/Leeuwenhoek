using System;
using BestHTTP.WebSocket;
using UnityEngine;

namespace MyWebSocket
{
    public class MyWebSocket : MonoBehaviour
    {
        public string url = "webSocket://localhost:8080/web1/webSocket";

        private WebSocket _webSocket;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _webSocket = new WebSocket(new Uri(url));
            _webSocket.OnOpen += OnOpen;
            _webSocket.OnMessage += OnMessageReceived;
            _webSocket.OnError += OnError;
            _webSocket.OnClosed += OnClosed;
            _webSocket.OnBinary += OnBinaryMessageReceived;
        }

        private void AntiInit()
        {
            _webSocket.OnOpen = null;
            _webSocket.OnMessage = null;
            _webSocket.OnError = null;
            _webSocket.OnClosed = null;
            _webSocket.OnBinary = null;
            _webSocket = null;
        }

        public void Connect()
        {
            _webSocket.Open();
        }

        public void Send(string str)
        {
            _webSocket.Send(str);
        }

        public void Close()
        {
            _webSocket.Close();
        }

        private void OnDestroy()
        {
            if (_webSocket is { IsOpen: true })
            {
                _webSocket.Close();
                AntiInit();
            }
        }


        #region WebSocket事件回调

        /// <summary>
        /// 在与服务器建立连接时调用
        /// 在此事件之后，WebSocket 的 IsOpen 属性将为 True
        /// </summary>
        private static void OnOpen(WebSocket webSocket)
        {
            Debug.Log("connected");
        }

        /// <summary>
        /// 在从服务器收到文本消息时调用
        /// </summary>
        private static void OnMessageReceived(WebSocket webSocket, string message)
        {
            Debug.Log(message);
        }

        /// <summary>
        /// 在从服务器二进制消息时调用
        /// </summary>
        private static void OnBinaryMessageReceived(WebSocket webSocket, byte[] message)
        {
            Debug.Log("Binary Message received from server. Length: " + message.Length);
        }

        /// <summary>
        /// 在与服务器关闭连接时调用
        /// </summary>
        private void OnClosed(WebSocket webSocket, ushort code, string message)
        {
            Debug.Log(message);
            AntiInit();
            Init();
        }


        /// <summary>
        /// 在无法连接到服务器、发生内部错误或连接丢失时调用
        /// </summary>
        private void OnError(WebSocket webSocket, string error)
        {
            var errorMsg = string.Empty;
#if !UNITY_WEBGL || UNITY_EDITOR
            if (webSocket.InternalRequest.Response != null)
            {
                errorMsg = $"Status Code from Server: {webSocket.InternalRequest.Response.StatusCode} and Message: {webSocket.InternalRequest.Response.Message}";
            }
#endif
            Debug.Log(errorMsg);
            AntiInit();
            Init();
        }

        #endregion
    }
}