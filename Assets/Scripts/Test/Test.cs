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
            RequestRegister requestRegister = new("nyc", "1234");
            Debug.Log(requestRegister.ToJson(true));

            RequestLogin requestLogin = new("nyc");
            Debug.Log(requestLogin.ToJson(true));

            RequestMessage requestMessage = new("sender", SendType.IndividualSending, "receiver", MessageType.Txt, "hello");
            Debug.Log(requestMessage.ToJson(true));
        }
    }
}