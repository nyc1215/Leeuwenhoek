using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace MyWebSocket.Response
{
    /// <summary>
    /// 同步数据响应
    /// </summary>
    public class ResponseSynchronousData : ResponseUtil
    {
        [JsonIgnore] public override string RequestID => null;

        [JsonIgnore] public List<Vector3>  playerPos {get
        {
            if (Data is List<Vector3> playerPosition)
            {
                return playerPosition;
            }
            return null;
        }}

        [JsonIgnore]
        public PlayerListData PlayerListData
        {
            get
            {
                if (Data is PlayerListData playerListData)
                {
                    return playerListData;
                }
                return null;
            }
        }

        public ResponseSynchronousData(string json) : base(json)
        {
        }
    }
}