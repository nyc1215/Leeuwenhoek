using System.Collections.Generic;
using Manager;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 登录请求
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RequestLogin : RequestUtil
    {
        [JsonProperty("data")] private RequestLoginData _data = new();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="account">账号</param>
        public RequestLogin(string account) : base(RequestType.Login)
        {
            _data.Account = account;
        }
        
        public override void CheckWorkDelegate(object data)
        {
            if ((string)data == "OK")
            {
                RequestSuccess();
            }
            else
            {
                RequestFail();
            }
            CleanWorkDelegate();
        }
    }
}