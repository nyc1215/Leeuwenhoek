using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyWebSocket.Request
{
    public enum MessageType
    {
        Txt,
        Voice
    }

    public sealed class RequestMessage : RequestUtil
    {
        [JsonIgnore] private string SenderID { get; set; }

        [JsonIgnore] private string ReceiverId { get; set; }

        [JsonIgnore] private string Message { get; set; }

        [JsonIgnore] private string Type { get; set; }


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
