using UnityEditor;
using UnityEngine;
namespace Pktony.GridInventory.Editor
{
    public static class InventoryAssetStore
    {
        public static T GetOrCreate<T>(string name) where T : ScriptableObject
        {
            string path = "Assets/ModularGridInventory/Samples/Catalog/" + name + ".asset"; var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>(); AssetDatabase.CreateAsset(asset, path); return asset;
        }
    }
}
