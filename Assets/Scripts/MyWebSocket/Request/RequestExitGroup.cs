using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    public class RequestExitGroup : RequestUtil
    {
        [JsonProperty("data")] private RequestAccountAndGroupIdData _data = new();

        public RequestExitGroup(string account, string groupId) : base(RequestType.ExitGroup)
        {
            _data.Account = account;
            _data.GroupId = groupId;
        }
    }
}