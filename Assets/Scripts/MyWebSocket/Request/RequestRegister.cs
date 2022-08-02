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
        private string _uid;
        private string _username;
        private string _playId;

        [JsonIgnore] public string Uid { get { return _uid; } set { _uid = value; } }
        [JsonIgnore] public string Username { get { return _username; } set { _username = value; } }
        [JsonIgnore] public string PlayId { get { return _playId; } set { _playId = value; } }

        public RequestRegister(string username, string playID) : base(RequestType.Register)
        {
            Uid = Guid.NewGuid().ToString("N");
            Username = username;
            PlayId = playID;
            Data = new Dictionary<string, string>()
            {
                {"uid" , Uid},
                {"accout_name" , Username},
                {"play_id" , PlayId}
            };
        }
    }
}
