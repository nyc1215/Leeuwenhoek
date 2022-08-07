using System.Collections.Generic;

namespace MyWebSocket.Response
{
    public struct PlayerListNode
    {
        public string Account;
        public string AccountName;
    }

    public struct PlayerListData
    {
        public string GroupId;
        public List<PlayerListNode> PlayerList;
    }
}