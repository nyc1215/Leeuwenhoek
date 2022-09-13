using System;
using System.Collections;
using System.Collections.Generic;
using UI.Game;
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

        private void OnEnable()
        {
            if (MyPlayerController.AllBodies != null)
            {
                MyPlayerController.AllBodies.Add(transform);
            }
        }

        public void Report()
        {
            Debug.Log("Reported");
            Destroy(gameObject);
        }
    }
}