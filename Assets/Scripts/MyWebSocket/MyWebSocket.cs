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

        private WebSocket _webSocket;

        public bool IsOpen => _webSocket.IsOpen;

        public void Connect()
        {
            _webSocket = new WebSocket(new Uri(uri));

            _webSocket.OnOpen += OnOpen;
            _webSocket.OnMessage += OnMessageReceived;
            _webSocket.OnError += OnError;
            _webSocket.OnClosed += OnClosed;
            _webSocket.OnBinary += OnBinaryMessageReceived;

            _webSocket.Open();
            Debug.Log("webSocket connecting...");
        }

        public void Send(string str)
        {
            _webSocket.Send(str);
        }

        public void Close()
        {
            _webSocket.Close(1000, "Bye!");
        }

        #region MonoBehaviour

        private void OnDestroy()
        {
            if (_webSocket != null)
            {
                _webSocket.Close();
                _webSocket = null;
            }
        }

        #endregion


        #region WebSocket事件回调

        /// <summary>
        /// 在与服务器建立连接时调用
        /// 在此事件之后，WebSocket 的 IsOpen 属性将为 True
        /// </summary>
        private static void OnOpen(WebSocket webSocket)
        {
            Debug.Log("WebSocket Open!");
        }

        /// <summary>
        /// 在从服务器收到文本消息时调用
        /// </summary>
        private static void OnMessageReceived(WebSocket webSocket, string message)
        {
            foreach (var player in MyGameManager.Instance.allPlayers)
            {
                player.GetResponse(message);
            }
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
            _webSocket = null;
        }


        /// <summary>
        /// 在无法连接到服务器、发生内部错误或连接丢失时调用
        /// </summary>
        private void OnError(WebSocket webSocket, string error)
        {
            Debug.Log($"An error occured: <color=red>{error}</color>");
            _webSocket = null;
        }

        #endregion
    }
}