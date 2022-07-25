using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace MyWeb
{
    /// <summary>
    /// 错误数据响应
    /// </summary>
    public class ResponseError : ResponseUtil
    {
        public ResponseError(string json) : base (json)
        {

        }
    }
}
