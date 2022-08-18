using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyWebSocket.Response
{
    public class PlayerListNode
    {
        [JsonProperty("account")] public string Account;
        [JsonProperty("accountName")] public string AccountName;
    }

    public class PlayerListData
    {
        [JsonProperty("groupId")] public string GroupId;
        [JsonProperty("playerList")] public List<PlayerListNode> PlayerList;
        [JsonProperty("scriptName")] public string ScriptName;
    }
    
}