using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    /// <summary>
    /// 请求体抽象基类
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class RequestUtil
    {
        /// <summary>
        /// 请求体类型
        /// </summary>
        [JsonProperty("requestType", Required = Required.Always, Order = -3)]
        [JsonConverter(typeof(RequestType))]
        private RequestType NowRequestType { get; set; }

        /// <summary>
        /// 唯一的请求标记，前端生成，后端保存
        /// </summary>
        [JsonProperty("requestId", Required = Required.Always, Order = -2)]
        public string RequestID { get; set; }

        protected RequestUtil(RequestType type)
        {
            NowRequestType = type;
            RequestID = Guid.NewGuid().ToString("N");
        }

        public string ToJson()
        {
            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.None
            };
            return JsonConvert.SerializeObject(this, setting);
        }

        public string ToJson(bool prettyPrint)
        {
            var setting = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = prettyPrint ? Formatting.Indented : Formatting.None
            };
            return JsonConvert.SerializeObject(this, setting);
        }
    }
}