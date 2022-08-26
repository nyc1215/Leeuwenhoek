using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    public class RequestCancelReady : RequestUtil
    {
        [JsonProperty("data")] private RequestAccountAndGroupIdData _data = new ();

        public RequestCancelReady(string account, string groupId) : base(RequestType.CancelReady)
        {
            _data.Account = account;
            _data.GroupId = groupId;
        }
    }
}