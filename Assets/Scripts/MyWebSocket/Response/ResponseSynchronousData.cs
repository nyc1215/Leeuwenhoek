using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Response
{
    /// <summary>
    /// 同步数据响应
    /// </summary>
    public class ResponseSynchronousData : ResponseUtil
    {
        [JsonIgnore] public override string RequestID => null;

        public Transform playerTransform { get; set; }

        public ResponseSynchronousData(string json) : base(json)
        {
        }

        public T GetData<T>() where T : struct
        {
            return default;
        }
    }
}