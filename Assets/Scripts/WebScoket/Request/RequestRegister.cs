using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
namespace MyWeb
{
    /// <summary>
    /// 注册请求
    /// </summary>
    public class RequestRegister : RequestUtil
    {
        private string _username;
        private string _password;

        [JsonIgnore] public string Username { get { return _username; } set { _username = value; } }
        [JsonIgnore] public string Password { get { return _password; } set { _password = value; } }

        public RequestRegister(string username, string password) : base(RequestType.Register)
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
