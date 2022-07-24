using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyWeb
{
    /// <summary>
    /// µÇÂ¼ÇëÇó
    /// </summary>
    public class RequestLogin : RequestUtil
    {
        private string _username;
        private string _password;

        public RequestLogin(string username, string password) : base(RequestType.Login)
        {
            _username = username;
            _password = password;
            data = new Dictionary<string, string>()
            {
                {"username", _username},
                {"password", _password}
            };
        }

    }
}
