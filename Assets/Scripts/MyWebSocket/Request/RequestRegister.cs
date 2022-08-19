using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 注册请求
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public sealed class RequestRegister : RequestUtil
    {
        [JsonProperty("data")]
        private RequestRegisterData _data = new();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="accountName">呢称，注册时要写</param>
        public RequestRegister(string account, string accountName) : base(RequestType.Register)
        {
            _data.Account = account;
            _data.AccountName = accountName;
        }

        public override void CheckWorkDelegate(object data)
        {
            if ((string)data == "OK")
            {
                Debug.Log("注册成功");
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