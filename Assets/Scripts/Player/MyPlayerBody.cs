using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    public class MyPlayerBody : MonoBehaviour
    {
        public SpriteRenderer bodySprite;

        public void SetColor(Color color)
        {
            bodySprite.color = color;
        }
    }
}