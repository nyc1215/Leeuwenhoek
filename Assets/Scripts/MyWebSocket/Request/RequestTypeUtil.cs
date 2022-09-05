using System.Collections;
using System.Collections.Generic;
using FairyGUI;
using Newtonsoft.Json;
using UnityEngine;

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

        /*取消匹配*/
        CancelMatching,

        /*创建房间*/
        CreateRoom,

        /*向别人发起组队邀请*/
        Invite,

        /*加入房间*/
        AddRoom,

        /*退出房间*/
        ExitGroup,

        /*提交线索*/
        SubmitLead,

        /*结算统计*/
        Settlement,

        /*提交剧本,选完剧本后提交*/
        SubmitScript,

        /*发消息，讨论的消息*/
        SendMessage,

        /*玩家准备*/
        Ready,
        
        CancelReady,
        
        /*开始游戏*/
        Start,

        /*玩家移动*/
        Move
    }

    public class RequestMatchData
    {
        [JsonProperty("account")] public string Account;
        [JsonProperty("scriptName")] public string ScriptName;
    }

    public class RequestLoginData
    {
        [JsonProperty("account")] public string Account;
    }

    public class RequestRegisterData
    {
        [JsonProperty("account")] public string Account;
        [JsonProperty("accountName")] public string AccountName;
    }

    public class RequestCreateRoomData
    {
        [JsonProperty("account")] public string Account;
    }

    public class RequestInviteData
    {
        [JsonProperty("account")] public string Account;
        [JsonProperty("accountName")] public string AccountName;
        [JsonProperty("targetAccount")] public string TargetAccount;
        [JsonProperty("groupId")] public string GroupId;
    }

    public class RequestAddRoomData
    {
        [JsonProperty("account")] public string Account;
        [JsonProperty("accountName")] public string AccountName;
        [JsonProperty("groupId")] public string GroupId;
    }

    public class RequestAccountAndGroupIdData
    {
        [JsonProperty("account")] public string Account;
        [JsonProperty("groupId")] public string GroupId;
    }

    public class RequestSendMessageData
    {
        [JsonProperty("sendAccount")] public string SendAccount;
        [JsonProperty("sendType")] public int SendType;
        [JsonProperty("receiverAccount")] public string ReceiverAccount;
        [JsonProperty("type")] public int Type;
        [JsonProperty("sendTime")] public string SendTime;
        [JsonProperty("content")] public byte[] Content;
    }

    /// <summary>
    /// 消息类型
    /// </summary>
    public enum MessageType
    {
        Txt,
        Voice
    }

    /// <summary>
    /// 发送类型
    /// </summary>
    public enum SendType
    {
        BatchSending,
        IndividualSending
    }
}