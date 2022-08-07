using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 申请创建房间请求
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RequestCreateRoom : RequestUtil
    {
        [JsonProperty("data", Required = Required.Always)]
        private RequestCreateRoomData _data;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="account">账号</param>
        public RequestCreateRoom(string account) : base(RequestType.CreateRoom)
        {
            _data.Account = account;
        }
    }
}