using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 加入房间请求
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class RequestAddRoom : RequestUtil
    {
        [JsonProperty("data", Required = Required.Always)]
        private RequestAddRoomData _data;

        public RequestAddRoom(string account, string accountName, string groupId) : base(RequestType.AddRoom)
        {
            _data.Account = account;
            _data.AccountName = accountName;
            _data.GroupId = groupId;
        }
    }
}