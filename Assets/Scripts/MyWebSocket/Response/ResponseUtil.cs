using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyWebSocket.Response
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
    /// "ResponseType"响应体类型
    /// "RequestId"对应的值是客户端向服务器发起请求时携带的RequestId（不是所有返回体都有这个）
    /// "Data"具体数据内容
    /// </summary>
    public class ResponseUtil
    {
        protected virtual ResponseType NowResponseType { get; set; }

        protected virtual string RequestID { get; set; }

        protected virtual Dictionary<string, object> Data { get; set; }

        protected ResponseUtil(string json)
        {
            var aResponse = JsonConvert.DeserializeObject<ResponseUtil>(json);
            NowResponseType = aResponse.NowResponseType;
            RequestID = aResponse.RequestID;
            Data = aResponse.Data;
        }

        /// <summary>
        /// 响应体解析Data中包含的数据
        /// </summary>
        /// <param name="target">解析后需要被赋值的对象</param>
        /// <param name="T">解析对象的类型</param>
        public bool ParseData<T>(ref T target)
        {
            if (Data.TryGetValue(target.ToString(), out var result))
            {
                if (result is T targetObj)
                {
                    target = targetObj;
                    return true;
                }
            }

            return false;
        }
    }
}