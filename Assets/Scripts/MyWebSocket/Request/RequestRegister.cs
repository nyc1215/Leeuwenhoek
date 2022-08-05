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
        [JsonIgnore] private string Uid { get; set; }

        [JsonIgnore] private string Username { get; set; }

        [JsonIgnore] private string PlayId { get; set; }

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
