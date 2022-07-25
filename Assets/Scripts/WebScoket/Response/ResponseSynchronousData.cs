using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
namespace MyWeb
{
    /// <summary>
    /// 同步数据响应
    /// </summary>
    /// <param = "PlayerPosition">玩家位置</param>
    public class ResponseSynchronousData : ResponseUtil
    {
        [JsonIgnore] public override string RequestID { get => base.RequestID; set => base.RequestID = value; }

        private Vector3 _playerPosition;

        public Vector3 PlayerPosition { get => _playerPosition; set => _playerPosition = value; }

        public ResponseSynchronousData(string json) : base(json)
        {

        }
    }
}
