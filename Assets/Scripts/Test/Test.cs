using MyWebSocket.Request;
using UnityEngine;

namespace Test
{
    public class Test : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            RequestRegister requestRegister = new("nyc","1234");
            Debug.Log(requestRegister.ToJson(true));

            RequestLogin requestLogin = new("nyc", "114514");
            Debug.Log(requestLogin.ToJson(true));

            RequestMessage requestMessage = new("senderId", "reveiverId", "Hello", MessageType.txt);
            Debug.Log(requestMessage.ToJson(true));
        }
    }
}
