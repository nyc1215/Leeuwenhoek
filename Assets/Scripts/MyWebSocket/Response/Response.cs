using MyWebSocket.Request;

namespace MyWebSocket.Response
{
    /// <summary>
    /// 请求数据响应
    /// </summary>
    public class Response : ResponseUtil
    {
        public Response(string json) : base(json)
        {
        }
    }
}