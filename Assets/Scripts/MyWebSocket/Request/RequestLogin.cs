using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 登录请求
    /// </summary>
    public sealed class RequestLogin : RequestUtil
    {


        [JsonProperty("Data")] private RequestLoginData _data;


        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="account">账号</param>
        public RequestLogin(string account) : base(RequestType.Login)
        {
            _data.Account = account;
        }
    }
}