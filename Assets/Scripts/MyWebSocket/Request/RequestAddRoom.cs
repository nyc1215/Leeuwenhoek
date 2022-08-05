using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    public class RequestAddRoom : RequestUtil
    {
        [JsonProperty("Data")] private RequestAddRoomData _data;

        public RequestAddRoom(string account, string accountName, string groupId) : base(RequestType.AddRoom)
        {
            _data.Account = account;
            _data.AccountName = accountName;
            _data.GroupId = groupId;
        }
    }
}