using System;
using System.Collections.Generic;
using Newtonsoft.Json;

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
    }
}