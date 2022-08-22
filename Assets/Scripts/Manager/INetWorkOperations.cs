using MyWebSocket.Request;
using MyWebSocket.Response;

namespace Manager
{
    public interface INetWorkOperations
    {
        public void SendRequest(RequestUtil request);
        public void GetResponse(string json);
        public void SynchronousData(string responseSynchronousData);
        public void ResponseErrorWork(ResponseUtil responseError);
        public void ResponseWork(ResponseUtil response);
    }
}