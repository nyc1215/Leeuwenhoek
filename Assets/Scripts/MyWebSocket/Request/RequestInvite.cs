using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 向别人发起组队邀请请求
    /// </summary>
    public class RequestInvite : RequestUtil
    {
        [JsonProperty("Data")] private RequestInviteData _data;

        public RequestInvite(string account, string accountName, string targetAccount, string groupId) : base(RequestType.Invite)
        {
            _data.Account = account;
            _data.AccountName = accountName;
            _data.TargetAccount = targetAccount;
            _data.GroupId = groupId;
        }
    }
}