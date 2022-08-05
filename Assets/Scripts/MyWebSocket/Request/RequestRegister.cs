using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 注册请求
    /// </summary>
    public sealed class RequestRegister : RequestUtil
    {

        [JsonProperty("Data")] private RequestRegisterData _data;

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