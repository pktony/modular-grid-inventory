using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InventorySystem.Utility
{
    public class ResourceUtility
    {
        public static T Load<T>(string path) where T : Object
        {
            T obj = Resources.Load<T>(path);
            return obj;
        }

        public static Sprite LoadSprite(string path)
        {
            Sprite sprite = Resources.Load<Sprite>($"Sprites/{path}");
            return sprite;
        }

        public static Sprite LoadItemSprite(ResourceType resourceType, string itemName)
        {
            Sprite sprite = Resources.Load<Sprite>($"{resourceType}/{itemName}");
            return sprite;
        }
    }
}