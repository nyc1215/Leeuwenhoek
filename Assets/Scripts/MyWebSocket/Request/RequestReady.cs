using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    public class RequestReady : RequestUtil
    {
        [JsonProperty("data")] private RequestAccountAndGroupIdData _data = new ();

        public RequestReady(string account, string groupId) : base(RequestType.Ready)
        {
            _data.Account = account;
            _data.GroupId = groupId;
        }
    }
}