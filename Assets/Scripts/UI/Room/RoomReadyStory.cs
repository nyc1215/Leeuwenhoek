using System.Collections.Generic;
using UnityEngine;

namespace UI.Room
{
    [CreateAssetMenu(menuName = "ScriptableSingleton/RoomReadyStory")]
    public class RoomReadyStory : ScriptableObject
    {
        public List<string> storyText = new();
    }
}