using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    public class RequestMatching : RequestUtil
    {
        [JsonProperty("data")] private RequestMatchData _data = new();


        public RequestMatching(string account, string scriptName) : base(RequestType.Matching)
        {
            _data.Account = account;
            _data.ScriptName = scriptName;
        }

        public override void CheckWorkDelegate(object data)
        {
            if ((string)data == "OK")
            {
                Debug.Log("匹配请求成功");
                RequestSuccess();
            }
            else
            {
                RequestFail();
            }

            CleanWorkDelegate();
        }
    }

    public class RequestCancelMatching : RequestUtil
    {
        [JsonProperty("data")] private RequestMatchData _data = new();

        public RequestCancelMatching(string account, string scriptName) : base(RequestType.CancelMatching)
        {
            _data.Account = account;
            _data.ScriptName = scriptName;
        }

        public override void CheckWorkDelegate(object data)
        {
            if ((string)data == "已经取消匹配")
            {
                Debug.Log("取消匹配成功");
                RequestSuccess?.Invoke();
            }
            else
            {
                RequestFail?.Invoke();
            }

            CleanWorkDelegate();
        }
    }
}