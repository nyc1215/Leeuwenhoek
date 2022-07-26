using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
namespace MyWeb
{
    public enum MessageType
    {
        txt,
        voice
    }

    public class RequestMessage : RequestUtil
    {
        private string _senderID;
        private string _receiverId;
        private string _message;
        private string _type;

        [JsonIgnore] public string SenderID { get => _senderID; set => _senderID = value; }
        [JsonIgnore] public string ReceiverId { get => _receiverId; set => _receiverId = value; }
        [JsonIgnore] public string Message { get => _message; set => _message = value; }
        [JsonIgnore] public string Type { get => _type; set => _type = value; }


        public RequestMessage(string senderID, string receiverId, string message, MessageType type) : base(RequestType.SendMessage)
        {
            SenderID = senderID;
            ReceiverId = receiverId;
            Message = message;
            Type = type.ToString();
            Data = new Dictionary<string, string>()
            {
                {"sender_id",SenderID },
                {"receiver_id",ReceiverId },
                {"content",Message},
                {"type",Type }
            };
        }
    }
}
