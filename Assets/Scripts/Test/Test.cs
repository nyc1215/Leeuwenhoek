using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyWeb;
public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RequestLogin requestLogin = new("nyc", "114514");
        Debug.Log(requestLogin.ToJson(true));
    }
}
