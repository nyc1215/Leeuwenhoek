using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

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
        public string NowRequestType { get; set; }

        /// <summary>
        /// 唯一的请求标记，前端生成，后端保存
        /// </summary>
        [JsonProperty("requestId", Required = Required.Always, Order = -2)]
        public string RequestID { get; set; }

        protected RequestUtil(RequestType type)
        {
            NowRequestType = type.ToString();
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
                Formatting = prettyPrint ? Formatting.Indented : Formatting.None,
                TypeNameHandling = TypeNameHandling.All
            };
            return JsonConvert.SerializeObject(this, setting);
        }

        /// <summary>
        /// 判断请求对应的响应是否成功
        /// 然后根据成功与否执行对应的委托
        /// </summary>
        public virtual void CheckWorkDelegate(object data)
        {
            if ((string)data == "OK")
            {
                Debug.Log("注册成功");
                RequestSuccess?.Invoke();
            }
            else
            {
                RequestFail?.Invoke();
            }
            CleanWorkDelegate();
        }

        protected void CleanWorkDelegate()
        {
            RequestSuccess = null;
            RequestFail = null;
        }
        
        public delegate void Work();

        /// <summary>
        /// 请求成功收到响应后执行的委托
        /// </summary>
        public Work RequestSuccess;

        /// <summary>
        /// 请求受到失败响应后执行的委托
        /// </summary>
        public Work RequestFail;
    }
}