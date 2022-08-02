using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 登录请求
    /// </summary>
    public class RequestLogin : RequestUtil
    {
        private string _username;
        private string _password;

        [JsonIgnore] public string Username { get { return _username; } set { _username = value; } }
        [JsonIgnore] public string Password { get { return _password; } set { _password = value; } }

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
