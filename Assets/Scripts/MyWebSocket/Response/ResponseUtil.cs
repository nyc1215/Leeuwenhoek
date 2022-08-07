using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
        [JsonProperty("responseType")] public string NowResponseType { get; set; }

        [JsonProperty("requestId")] public virtual string RequestID { get; set; }

        [JsonProperty("data")] protected object Data { get; set; }

        [JsonConstructor]
        public ResponseUtil(string json)
        {
            var aResponse = JsonConvert.DeserializeObject<ResponseUtil>(json);
            NowResponseType = aResponse.NowResponseType;
            RequestID = aResponse.RequestID;
            Data = aResponse.Data;
        }
    }
}