using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;
namespace MyWeb
{
    /// <summary>
    /// 响应类型
    /// </summary>
    public enum ResponseType
    {
        /*玩家同步操作*/
        SynchronousData,
        /*针对请求的数据返回*/
        Response,
        /*异常信息返回*/
        Error
    }

    /// <summary>
    /// 响应体抽象基类
    /// </summary>
    /// <param = "ResponseType">响应体类型</param>
    /// <param = "RequestId">对应的值是客户端向服务器发起请求时携带的RequestId（不是所有返回体都有这个）</param>
    /// <param = "Data">具体数据内容</param>
    public class ResponseUtil
    {
        private ResponseType _responseType;
        private string _requestID;
        private Dictionary<string, object> _data;

        public virtual ResponseType NowResponseType { get { return _responseType; } set { _responseType = value; } }
        public virtual string RequestID { get { return _requestID; } set { _requestID = value; } }
        public virtual Dictionary<string, object> Data { get { return _data; } set { _data = value; } }

        protected ResponseUtil(string json)
        {
            var a_Response = JsonConvert.DeserializeObject<ResponseUtil>(json);
            NowResponseType = a_Response.NowResponseType;
            RequestID = a_Response.RequestID;
            Data = a_Response.Data;
        }

        /// <summary>
        /// 响应体解析Data中包含的数据
        /// </summary>
        /// <param = "target">解析后需要被赋值的对象</param>
        /// <param = "T">解析对象的类型</param>
        public bool ParseData<T>(ref T target)
        {
            if (Data.TryGetValue(target.ToString(), out object result))
            {
                if (result is T targetOBJ)
                {
                    target = targetOBJ;
                    return true;
                }
            }
            return false;
        }
    }
}
