using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    public class RequestStart : RequestUtil
    {
        [JsonProperty("data")] private RequestAccountAndGroupIdData _data = new();

        protected RequestStart(string account, string groupId) : base(RequestType.Start)
        {
            _data.Account = account;
            _data.GroupId = groupId;
        }
    }
}