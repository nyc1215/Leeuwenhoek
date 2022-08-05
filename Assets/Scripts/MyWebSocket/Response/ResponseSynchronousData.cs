using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Response
{
    /// <summary>
    /// 同步数据响应
    /// </summary>
    public class ResponseSynchronousData : ResponseUtil
    {
        [JsonIgnore]
        protected override string RequestID => null;

        public Vector3 PlayerPosition { get; set; }

        public ResponseSynchronousData(string json) : base(json)
        {
        }
    }
}