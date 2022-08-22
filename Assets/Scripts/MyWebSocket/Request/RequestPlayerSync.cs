using System.Collections;
using System.Collections.Generic;
using MyWebSocket.Request;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Request
{
    public class RequestPlayerSync : RequestUtil
    {
        protected RequestPlayerSync(RequestType type) : base(type)
        {
        }
    }
}