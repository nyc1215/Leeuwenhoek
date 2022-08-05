using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 发消息请求
    /// </summary>
    public sealed class RequestMessage : RequestUtil
    {
        [JsonProperty("data")] private RequestSendMessageData _data;

        public RequestMessage(string sendAccount, SendType sendType, string receiveAccount, MessageType type, string content) : base(RequestType.SendMessage)
        {
            _data.SendAccount = sendAccount;
            _data.SendType = (int)sendType;
            _data.ReceiverAccount = receiveAccount;
            _data.Type = (int)type;
            _data.Content = System.Text.Encoding.UTF8.GetBytes(content);
            _data.SendTime = DateTime.Now.ToString("f");
        }
    }
}