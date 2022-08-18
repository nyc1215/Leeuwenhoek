using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    public class RequestMatching : RequestUtil
    {
        
        [JsonProperty("data")] private RequestMatchData _data = new();
       

        protected RequestMatching() : base(RequestType.Matching)
        {
        }
        
        public override void CheckWorkDelegate(object data)
        {
            if ((string)data == "OK")
            {
                RequestSuccess();
            }
            else
            {
                RequestFail();
            }
            CleanWorkDelegate();
        }
    }
}