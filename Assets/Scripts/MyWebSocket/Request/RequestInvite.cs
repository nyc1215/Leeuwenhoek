using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 向别人发起组队邀请请求
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class RequestInvite : RequestUtil
    {
        [JsonProperty("data")]
        private RequestInviteData _data = new();

        public RequestInvite(string account, string accountName, string targetAccount, string groupId) : base(
            RequestType.Invite)
        {
            _data.Account = account;
            _data.AccountName = accountName;
            _data.TargetAccount = targetAccount;
            _data.GroupId = groupId;
        }

        public override void CheckWorkDelegate(object data)
        {
            if ((string)data == "OK")
            {
                Debug.Log("邀请成功");
                RequestSuccess?.Invoke();
            }
            else
            {
                RequestFail?.Invoke();
            }
            CleanWorkDelegate();
        }
    }
}