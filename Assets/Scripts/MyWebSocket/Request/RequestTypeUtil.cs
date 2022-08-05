using System.Collections;
using System.Collections.Generic;
using FairyGUI;
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

        /*创建房间*/
        CreateRoom,

        /*向别人发起组队邀请*/
        Invite,

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

    public struct RequestLoginData
    {
        public string Account;
    };

    public struct RequestRegisterData
    {
        public string Account;
        public string AccountName;
    }

    public struct RequestCreateRoomData
    {
        public string Account;
    }

    public struct RequestInviteData
    {
        public string Account;
        public string AccountName;
        public string TargetAccount;
        public string GroupId;
    }

    public struct RequestAddRoomData
    {
        public string Account;
        public string AccountName;
        public string GroupId;
    }

    public struct RequestSendMessageData
    {
        public string SendAccount;
        public int SendType;
        public string ReceiverAccount;
        public int Type;
        public string SendTime;
        public byte[] Content;
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