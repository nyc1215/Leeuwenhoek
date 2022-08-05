using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 登录请求
    /// </summary>
    public sealed class RequestLogin : RequestUtil
    {
        [JsonIgnore] private string Username { get; set; }
        [JsonIgnore] private string Password { get; set; }

        public RequestLogin(string username, string password) : base(RequestType.Login)
        {
            Username = username;
            Password = password;
            Data = new Dictionary<string, string>()
            {
                {"username", Username},
                {"password", Password}
            };
        }

    }
}
