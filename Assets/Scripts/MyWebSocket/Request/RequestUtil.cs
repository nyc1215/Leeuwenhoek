using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    public enum RequestType
    {
        /*登录请求*/
        Login,

        /*注册请求*/
        Register,

        /*请求获取剧本列表*/
        ListOfScripts,

        /*匹配，提交用户信息至玩家池内*/
        Matching,

        /*创建房间*/
        CreateRoom,

        /*加入房间*/
        AddRoom,

        /*提交线索*/
        SubmitLead,

        /*结算统计*/
        Settlement,

        /*提交剧本,选完剧本后提交*/
        SubmitScript,

        /*发消息，讨论的消息*/
        SendMessage,
    }

    /// <summary>
    /// 请求体抽象基类
    /// </summary>
    public class RequestUtil
    {
        /// <summary>
        /// 请求体类型
        /// </summary>
        private string NowRequestType { get; set; }

        /// <summary>
        /// 唯一的请求标记，前端生成，后端保存
        /// </summary>
        private string RequestID { get; set; }

        /// <summary>
        /// 具体数据内容
        /// </summary>
        protected Dictionary<string, string> Data { get; set; }

        protected RequestUtil(RequestType type)
        {
            NowRequestType = type.ToString();
            RequestID = Guid.NewGuid().ToString("N");
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.None);
        }

        public string ToJson(bool prettyPrint)
        {
            return JsonConvert.SerializeObject(this, prettyPrint ? Formatting.Indented : Formatting.None);
        }
    }
}