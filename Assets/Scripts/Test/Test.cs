using MyWebSocket.Request;
using UnityEngine;

namespace Test
{
    /// <summary>
    /// 用于与服务器进行通信测试的类
    /// </summary>
    public class Test : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log(new RequestLogin("nyc").ToJson(true));
            Debug.Log(new RequestRegister("nyc", "1234").ToJson(true));
            Debug.Log(new RequestCreateRoom("nyc").ToJson(true));
            Debug.Log(new RequestInvite("nyc", "一超", "12345", "538097635077128192").ToJson(true));
            Debug.Log(new RequestAddRoom("nyc", "一超", "538097635077128192").ToJson(true));
            Debug.Log(new RequestMessage("sender", SendType.IndividualSending, "receiver", MessageType.Txt, "hello")
                .ToJson(true));
        }
    }
}