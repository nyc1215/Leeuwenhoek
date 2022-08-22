using MyWebSocket.Request;
using Newtonsoft.Json;
using UnityEngine;

namespace Test
{
    /// <summary>
    /// 用于与服务器进行通信测试的类
    /// </summary>
    public class Test : MonoBehaviour
    {
        public GameObject cube;

        public struct PlayerPosition
        {
            [JsonProperty("x")] private float _x;
            [JsonProperty("y")] private float _y;
            [JsonProperty("z")] private float _z;

            public PlayerPosition(Vector3 vector3)
            {
                _x = vector3.x;
                _y = vector3.y;
                _z = vector3.z;
            }
        }

        private void Start()
        {
            Debug.Log(new RequestLogin("nyc").ToJson(true));
            Debug.Log(new RequestRegister("nyc", "1234").ToJson(true));
            Debug.Log(new RequestCreateRoom("nyc").ToJson(true));
            Debug.Log(new RequestInvite("nyc", "一超", "12345", "538097635077128192").ToJson(true));
            Debug.Log(new RequestAddRoom("nyc", "一超", "538097635077128192").ToJson(true));
            Debug.Log(new RequestMessage("sender", SendType.IndividualSending, "receiver", MessageType.Txt, "hello")
                .ToJson(true));


            Debug.Log(JsonConvert.SerializeObject(new PlayerPosition(cube.transform.position), Formatting.Indented));
        }
    }
}