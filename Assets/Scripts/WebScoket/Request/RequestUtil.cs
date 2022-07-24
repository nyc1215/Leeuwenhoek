using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 请求类型
/// </summary>
namespace MyWeb
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
        SubmitScript
    }

    /// <summary>
    /// 请求体抽象基类
    /// </summary>
    /// <param = "RequestType">请求体类型</param>
    /// <param = "RequestId">是唯一的请求标记，前端生成，后端保存</param>
    /// <param = "Data">具体数据内容</param>
    public class RequestUtil
    {
        public string requestType;
        public string requestID;
        public Dictionary<string, string> data;

        protected RequestUtil(RequestType type)
        {
            requestType = type.ToString();
            requestID = Guid.NewGuid().ToString("N");
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