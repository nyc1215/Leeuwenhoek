﻿using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyWebSocket.Response
{
    public class PlayerListNode
    {
        [JsonProperty("account")] public string Account;
        [JsonProperty("accountName")] public string AccountName;
        [JsonProperty("status")] public string Status;
    }

    public class PlayerListData
    {
        [JsonProperty("groupId")] public string GroupId;
        [JsonProperty("playerList")] public List<PlayerListNode> PlayerList;
        [JsonProperty("scriptName")] public string ScriptName;
    }

    public class PlayerRoomStatusData
    {
        [JsonProperty("account")] public string Account;
        [JsonProperty("accountName")] public string AccountName;
        [JsonProperty("status")] public string Status;
    }
}