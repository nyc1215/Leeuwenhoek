using System.Collections;
using System.Collections.Generic;
using Manager;
using UnityEngine;
using MyWebSocket;
using MyWebSocket.Request;
using MyWebSocket.Response;

namespace Player
{
    public class MyPlayer : MonoBehaviour
    {
        private readonly Hashtable _requestPool = new();

        public void SendRequest<T>(T request) where T : RequestUtil
        {
            var t = typeof(T);
            if (MyWebSocket.MyWebSocket.Instance.IsOpen)
            {
                MyWebSocket.MyWebSocket.Instance.Send(request.ToJson());
                if (t == typeof(RequestInvite) || t == typeof(RequestAddRoom))
                {
                    return;
                }

                _requestPool.Add(request.RequestID, request);
            }
            else
            {
                Debug.LogWarning("Sever is not connected!!!");
            }
        }

        public void GetResponse(string json)
        {
            ResponseUtil responseUtil = new(json);
            switch (responseUtil.NowResponseType)
            {
                case ResponseType.Error:
                    ResponseErrorWork((ResponseError)responseUtil);
                    break;
                case ResponseType.Response:
                    ResponseWork((Response)responseUtil);
                    break;
                case ResponseType.SynchronousData:
                    SynchronousData((ResponseSynchronousData)responseUtil);
                    break;
                default:
                    throw new System.InvalidCastException();
            }
        }

        private void SynchronousData(ResponseSynchronousData responseSynchronousData)
        {
        }

        private void ResponseErrorWork(ResponseError responseError)
        {
            if (!_requestPool.ContainsKey(responseError.RequestID))
            {
                return;
            }
            
            _requestPool.Remove(responseError.RequestID);
        }

        private void ResponseWork(Response response)
        {
            if (!_requestPool.ContainsKey(response.RequestID))
            {
                return;
            }
            
            _requestPool.Remove(response.RequestID);
        }
    }
}