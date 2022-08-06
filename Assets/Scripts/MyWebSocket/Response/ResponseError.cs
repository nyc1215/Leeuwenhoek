namespace MyWebSocket.Response
{
    /// <summary>
    /// 错误数据响应
    /// </summary>
    public class ResponseError : ResponseUtil
    {
        public ResponseError(string json) : base(json)
        {
        }

        public string GetData()
        {
            return (string)Data;
        }
    }
}